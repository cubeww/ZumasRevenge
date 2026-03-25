using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Drivers.Graphics;

public class BaseXNARenderDevice : RenderDevice3D
{
	private const int PhysicalBackBufferWidth = 800;

	private const int PhysicalBackBufferHeight = 480;

	public enum ClipperType
	{
		Clipper_Less,
		Clipper_Greater,
		Clipper_Equal,
		Clipper_GreaterEqual,
		Clipper_LessEqual
	}

	public GraphicsDeviceManager mDevice;

	protected BasicEffect mBasicEffect;

	protected AlphaTestEffect mAlphaTestEffect;

	public BaseXNAStateManager mStateMgr;

	public BlendState mBlendState;

	public Matrix mProjectionMatrix;

	public Matrix mViewMatrix;

	private float mPixelOffset;

	private int mMinTextureWidth;

	private int mMinTextureHeight;

	private int mMaxTextureWidth;

	private int mMaxTextureHeight;

	private int mMaxTextureAspectRatio;

	private uint mRenderModeFlags;

	public uint mSupportedTextureFormats;

	public bool mTextureSizeMustBePow2;

	public bool mRenderTargetMustBePow2;

	private ulong mDefaultVertexSize;

	public ulong mDefaultVertexFVF;

	private int mWidth;

	private int mHeight;

	private int mScreenWidth;

	private int mScreenHeight;

	private bool mSceneBegun;

	private VertexPositionColorTexture[] mBatchedTriangleBuffer;

	private short[] mBatchedIndexBuffer;

	private int mBatchedTriangleIndex;

	private int mBatchedIndexIndex;

	private static int mBatchedTriangleSize = 1200;

	private IGraphicsDriver mGraphicsDriver;

	private Texture2D mTexture;

	private Game mGame;

	public Image mImage;

	public Transform mTransform = new Transform();

	private HRenderContext mCurrentContex;

	private HRenderContext GlobalRenderContex;

	private Stack<SexyTransform2D> mTransformStack;

	private static bool SUPPORT_HW_CLIP = false;

	public SpriteBatch mSpriteBatch;

	public VertexPositionColorTexture[] mTmpVPCTBuffer;

	public VertexPositionColor[] mTmpVPCBuffer;

	private BlendState mNormalState = new BlendState();

	private BlendState mAdditiveState = new BlendState();

	private int mCurDrawMode;

	private RenderTarget2D mScreenTarget;

	public Rectangle mRenderRect = new Rectangle(0, 0, 800, 480);

	private void UpdatePresentationRect()
	{
		int num = Math.Max(mDevice.PreferredBackBufferWidth, 1);
		int num2 = Math.Max(mDevice.PreferredBackBufferHeight, 1);
		int num3 = Math.Max(mWidth, 1);
		int num4 = Math.Max(mHeight, 1);
		float num5 = Math.Min((float)num / (float)num3, (float)num2 / (float)num4);
		int num6 = Math.Max(1, (int)Math.Round((float)num3 * num5));
		int num7 = Math.Max(1, (int)Math.Round((float)num4 * num5));
		mRenderRect = new Rectangle((num - num6) / 2, (num2 - num7) / 2, num6, num7);
	}

	public void ResizeBackBuffer(int width, int height)
	{
		width = Math.Max(width, 1);
		height = Math.Max(height, 1);
		if (mDevice.PreferredBackBufferWidth == width && mDevice.PreferredBackBufferHeight == height)
		{
			return;
		}
		mDevice.IsFullScreen = false;
		mDevice.PreferredBackBufferWidth = width;
		mDevice.PreferredBackBufferHeight = height;
		mScreenWidth = width;
		mScreenHeight = height;
		mDevice.ApplyChanges();
		UpdatePresentationRect();
		if (mCurrentContex == null)
		{
			SetViewport(mRenderRect.X, mRenderRect.Y, mRenderRect.Width, mRenderRect.Height, 0f, 1f);
		}
	}

	public BaseXNARenderDevice(IGraphicsDriver theDriver)
	{
		mDevice = new GraphicsDeviceManager((theDriver as XNAGraphicsDriver).GetMainGame());
		mDevice.IsFullScreen = false;
		mDevice.PreferredBackBufferWidth = PhysicalBackBufferWidth;
		mDevice.PreferredBackBufferHeight = PhysicalBackBufferHeight;
		mDevice.SynchronizeWithVerticalRetrace = false;
		mGame = (theDriver as XNAGraphicsDriver).GetMainGame();
		mStateMgr = new BaseXNAStateManager(ref mDevice);
		mDevice.SynchronizeWithVerticalRetrace = false;
		mTransformStack = new Stack<SexyTransform2D>();
		mBatchedTriangleBuffer = new VertexPositionColorTexture[mBatchedTriangleSize];
		mCurrentContex = null;
	}

	public BaseXNARenderDevice(Game game)
	{
		mGame = game;
		mDevice = new GraphicsDeviceManager(game);
		mDevice.IsFullScreen = false;
		mDevice.PreferredBackBufferWidth = PhysicalBackBufferWidth;
		mDevice.PreferredBackBufferHeight = PhysicalBackBufferHeight;
		mDevice.SynchronizeWithVerticalRetrace = false;
		mStateMgr = new BaseXNAStateManager(ref mDevice);
		mTransformStack = new Stack<SexyTransform2D>();
		mBatchedTriangleBuffer = new VertexPositionColorTexture[mBatchedTriangleSize];
		mCurrentContex = null;
	}

	public void Init()
	{
		SetViewport(mRenderRect.X, mRenderRect.Y, mRenderRect.Width, mRenderRect.Height, 0f, 1f);
	}

	public override RenderDevice3D Get3D()
	{
		return this;
	}

	public override bool CanFillPoly()
	{
		return true;
	}

	public override HRenderContext CreateContext(Image theDestImage, HRenderContext theSourceContext)
	{
		if (theSourceContext == null)
		{
			RenderTarget2D renderTarget2D = null;
			if (theDestImage != null)
			{
				HRenderContext hRenderContext = new HRenderContext();
				XNATextureData xNATextureData = (XNATextureData)theDestImage.GetRenderData();
				if (xNATextureData != null && xNATextureData.mTextures[0].mTexture != null)
				{
					renderTarget2D = (RenderTarget2D)xNATextureData.mTextures[0].mTexture;
				}
				else
				{
					renderTarget2D = new RenderTarget2D(mDevice.GraphicsDevice, theDestImage.GetWidth(), theDestImage.GetHeight());
					XNATextureData xNATextureData2 = new XNATextureData(null);
					theDestImage.SetRenderData(xNATextureData2);
					xNATextureData2.mWidth = renderTarget2D.Width;
					xNATextureData2.mHeight = renderTarget2D.Height;
					xNATextureData2.mTexPieceWidth = renderTarget2D.Width;
					xNATextureData2.mTexPieceHeight = renderTarget2D.Height;
					xNATextureData2.mTexVecWidth = 1;
					xNATextureData2.mTexVecHeight = 1;
					xNATextureData2.mPixelFormat = PixelFormat.PixelFormat_A8R8G8B8;
					xNATextureData2.mMaxTotalU = 1f;
					xNATextureData2.mMaxTotalV = 1f;
					xNATextureData2.mImageFlags = theDestImage.GetImageFlags();
					xNATextureData2.mOptimizedLoad = true;
					xNATextureData2.mTextures[0].mWidth = renderTarget2D.Width;
					xNATextureData2.mTextures[0].mHeight = renderTarget2D.Height;
					xNATextureData2.mTextures[0].mTexture = renderTarget2D;
				}
				hRenderContext.mHandlePtr = renderTarget2D;
				return hRenderContext;
			}
			return null;
		}
		return theSourceContext;
	}

	public override void DeleteContext(HRenderContext theContext)
	{
	}

	public override void SetCurrentContext(HRenderContext theContext)
	{
		if (theContext != mCurrentContex)
		{
			if (mBatchedTriangleIndex > 0)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
			if (theContext == null || theContext.GetPointer() == null)
			{
				mDevice.GraphicsDevice.SetRenderTarget(null);
				mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, mWidth, mHeight, 0f, -1000f, 1000f));
				SetViewport(mRenderRect.X, mRenderRect.Y, mRenderRect.Width, mRenderRect.Height, 0f, 1f);
				mCurrentContex = theContext;
			}
			else
			{
				RenderTarget2D renderTarget2D = theContext.GetPointer() as RenderTarget2D;
				mDevice.GraphicsDevice.SetRenderTarget(renderTarget2D);
				mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, renderTarget2D.Width, renderTarget2D.Height, 0f, -1000f, 1000f));
				mCurrentContex = theContext;
			}
		}
	}

	public override HRenderContext GetCurrentContext()
	{
		return mCurrentContex;
	}

	public override void PushState()
	{
		mStateMgr.PushState();
	}

	public override void PopState()
	{
		mStateMgr.PopState();
	}

	public override int Flush(uint inFlushFlags)
	{
		DoCommitAllRenderState();
		FlushBufferedTriangles();
		return 0;
	}

	public override void SetRenderRect(int theX, int theY, int theWidth, int theHeight)
	{
		mRenderRect = new Rectangle(theX, theY, theWidth, theHeight);
	}

	public override int Present(Rect theSrcRect, Rect theDestRect)
	{
		if (mBatchedTriangleIndex > 0)
		{
			DoCommitAllRenderState();
			FlushBufferedTriangles();
		}
		return 0;
	}

	public override uint GetCapsFlags()
	{
		return 0u;
	}

	public override uint GetMaxTextureStages()
	{
		return 0u;
	}

	public override string GetInfoString(EInfoString inInfoStr)
	{
		return "";
	}

	public override void GetBackBufferDimensions(ref uint outWidth, ref uint outHeight)
	{
	}

	public override int SceneBegun()
	{
		return 0;
	}

	public override bool CreateImageRenderData(ref MemoryImage inImage)
	{
		if (inImage != null && inImage.mRenderData != null)
		{
			XNATextureData xNATextureData = (XNATextureData)inImage.GetRenderData();
			if (xNATextureData.mOptimizedLoad)
			{
				xNATextureData.mImageFlags = inImage.GetImageFlags();
			}
			return true;
		}
		if (inImage != null)
		{
			SharedImageRef sharedImageRef = GlobalMembers.gSexyApp.mResourceManager.LoadImage(inImage.mNameForRes);
			inImage.Dispose();
			inImage = null;
			inImage = sharedImageRef.GetMemoryImage();
			if (inImage != null && inImage.mRenderData != null)
			{
				return true;
			}
		}
		return false;
	}

	public override void RemoveImageRenderData(MemoryImage img)
	{
		if (!(img.GetRenderData() is XNATextureData xNATextureData))
		{
			return;
		}
		for (int i = 0; i < xNATextureData.mTextures.Length; i++)
		{
			if (xNATextureData.mTextures[i] != null && xNATextureData.mTextures[i].mTexture != null && mStateMgr.mLastXNATextureSlots == xNATextureData.mTextures[i].mTexture)
			{
				mStateMgr.mLastXNATextureSlots = null;
			}
		}
		xNATextureData.Dispose();
		img.SetRenderData(null);
	}

	public override int RecoverImageBitsFromRenderData(MemoryImage inImage)
	{
		return 0;
	}

	public override int GetTextureMemorySize(MemoryImage theImage)
	{
		return 0;
	}

	public override PixelFormat GetTextureFormat(MemoryImage theImage)
	{
		return PixelFormat.PixelFormat_A4R4G4B4;
	}

	public override void AdjustVertexUVsEx(uint theVertexFormat, SexyVertex[] theVertices, int theVertexCount, int theVertexSize)
	{
	}

	public void AdjustVertsForAtlas(int inTextureIndex, ref VertexPositionColorTexture[] inVerts, int inStartIndex, int inVertCount, uint inVertFormat, int inStride, int inTexUVOfs)
	{
	}

	public Image SetupAtlasState(int inTextureIndex, Image inImage)
	{
		if (inImage == null)
		{
			return null;
		}
		if (inImage.mAtlasImage != null)
		{
			return inImage.mAtlasImage;
		}
		return inImage;
	}

	public override void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, SexyFramework.Graphics.Color theColor, int theDrawMode, float tx, float ty, bool blend, uint theFlags)
	{
		int num = 0;
		switch (thePrimitiveType)
		{
		case Graphics3D.EPrimitiveType.PT_PointList:
			num = thePrimitiveCount;
			break;
		case Graphics3D.EPrimitiveType.PT_LineList:
			num = thePrimitiveCount * 2;
			break;
		case Graphics3D.EPrimitiveType.PT_LineStrip:
			num = 1 + thePrimitiveCount;
			break;
		case Graphics3D.EPrimitiveType.PT_TriangleList:
			num = thePrimitiveCount * 3;
			break;
		case Graphics3D.EPrimitiveType.PT_TriangleStrip:
			num = 2 + thePrimitiveCount;
			break;
		case Graphics3D.EPrimitiveType.PT_TriangleFan:
			num = 2 + thePrimitiveCount;
			break;
		}
		if (num == 0 || thePrimitiveCount == 0 || !PreDraw())
		{
			return;
		}
		mStateMgr.PushState();
		Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
		SetupDrawMode(theDrawMode);
		mImage.InitAtalasState();
		VertexPositionColorTexture[] array = new VertexPositionColorTexture[num];
		if ((theVertexFormat & 4) != 0 && (color.PackedValue != 0 || tx != 0f || ty != 0f || mTransformStack.Count != 0))
		{
			for (int i = 0; i < num; i++)
			{
				theVertices[i].x += tx;
				theVertices[i].y += ty;
				if (theVertices[i].color == SexyFramework.Graphics.Color.Zero)
				{
					theVertices[i].color = theColor;
				}
				if (mTransformStack.Count != 0)
				{
					SexyVector2 sexyVector = new SexyVector2(theVertices[i].x, theVertices[i].y);
					sexyVector = mTransformStack.Peek() * sexyVector;
					theVertices[i].x = sexyVector.x;
					theVertices[i].y = sexyVector.y;
				}
			}
		}
		for (int j = 0; j < num; j++)
		{
			array[j].Position.X = theVertices[j].x;
			array[j].Position.Y = theVertices[j].y;
			array[j].Position.Z = 0f;
			array[j].TextureCoordinate = mImage.mVectorBase + mImage.mVectorU * theVertices[j].u + mImage.mVectorV * theVertices[j].v;
			if (theVertices[j].color == SexyFramework.Graphics.Color.Zero)
			{
				array[j].Color = color;
			}
			else
			{
				array[j].Color = GetXNAColor(theVertices[j].color);
			}
		}
		mStateMgr.SetWorldTransform(Matrix.Identity);
		mStateMgr.mStateDirty = true;
		DrawPrimitiveInternal((int)thePrimitiveType, thePrimitiveCount, array, 0uL, theVertexFormat, inDoCommit: true, Matrix.Identity);
		mStateMgr.mStateDirty = false;
		mStateMgr.PopState();
	}

	public override void SetBltDepth(float inDepth)
	{
	}

	public override void PushTransform(SexyMatrix3 theTransform, bool concatenate)
	{
		if (mTransformStack.Count == 0 || !concatenate)
		{
			mTransformStack.Push((SexyTransform2D)(object)theTransform);
			return;
		}
		SexyTransform2D sexyTransform2D = mTransformStack.Pop();
		mTransformStack.Push(sexyTransform2D * (SexyTransform2D)(object)theTransform);
	}

	public override void PopTransform()
	{
		if (mTransformStack.Count != 0)
		{
			mTransformStack.Pop();
		}
	}

	public override void PopTransform(ref SexyMatrix3 theTransform)
	{
		if (mTransformStack.Count != 0)
		{
			theTransform = mTransformStack.Pop();
			return;
		}
		SexyTransform2D sexyTransform2D = default(SexyTransform2D);
		sexyTransform2D.LoadIdentity();
		theTransform = sexyTransform2D;
	}

	public override void ClearColorBuffer(SexyFramework.Graphics.Color inColor)
	{
		mDevice.GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(inColor.mRed, inColor.mGreen, inColor.mBlue, inColor.mAlpha));
	}

	public override void ClearStencilBuffer(int inStencil)
	{
		mDevice.GraphicsDevice.Clear(ClearOptions.Stencil, Microsoft.Xna.Framework.Color.White, 0f, 0);
	}

	public override void SetMaterialAmbient(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
	{
	}

	public override void SetMaterialDiffuse(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
	{
	}

	public override void SetMaterialSpecular(SexyFramework.Graphics.Color inColor, int inVertexColorComponent, float inPower)
	{
	}

	public override void SetMaterialEmissive(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
	{
	}

	public override void SetWorldTransform(SexyMatrix4 inMatrix)
	{
		mStateMgr.SetWorldTransform(GetXNAMatrix(inMatrix));
	}

	public override void SetViewTransform(SexyMatrix4 inMatrix)
	{
		mStateMgr.SetViewTransform(GetXNAMatrix(inMatrix));
	}

	public override void SetProjectionTransform(SexyMatrix4 inMatrix)
	{
		mStateMgr.SetProjectionTransform(GetXNAMatrix(inMatrix));
	}

	public override void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix, int inNumDimensions)
	{
	}

	public override void SetTextureWrap(int inTextureIndex, bool inWrapU, bool inWrapV)
	{
		if (inWrapU || inWrapV)
		{
			mStateMgr.SetSamplerState(SamplerState.LinearWrap);
		}
		else
		{
			mStateMgr.SetSamplerState(SamplerState.LinearClamp);
		}
	}

	public override void SetTextureLinearFilter(int inTextureIndex, bool inLinear)
	{
		if (!inLinear)
		{
			mStateMgr.SetSamplerState(SamplerState.PointClamp);
		}
		else
		{
			mStateMgr.SetSamplerState(SamplerState.LinearClamp);
		}
	}

	public override void SetTextureCoordSource(int inTextureIndex, int inUVComponent, Graphics3D.ETexCoordGen inTexGen)
	{
	}

	public override void SetTextureFactor(int inTextureFactor)
	{
	}

	public override void ClearDepthBuffer()
	{
	}

	public override void SetStencilState(Graphics3D.ECompareFunc inStencilTestFunc, int inRefStencil, bool inStencilEnable, Graphics3D.ETestResultFunc passFunc, Graphics3D.ETestResultFunc failFunc)
	{
		DepthStencilState mXNADepthStencilState = mStateMgr.mXNADepthStencilState;
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = mXNADepthStencilState.DepthBufferEnable;
		depthStencilState.DepthBufferFunction = mXNADepthStencilState.DepthBufferFunction;
		depthStencilState.DepthBufferWriteEnable = mXNADepthStencilState.DepthBufferWriteEnable;
		depthStencilState.StencilEnable = inStencilEnable;
		depthStencilState.ReferenceStencil = inRefStencil;
		depthStencilState.StencilFunction = GetXNACompareFunc(inStencilTestFunc);
		depthStencilState.StencilPass = (StencilOperation)passFunc;
		depthStencilState.StencilFail = (StencilOperation)failFunc;
		mStateMgr.SetDepthStencilState(depthStencilState);
	}

	public override void SetDepthState(Graphics3D.ECompareFunc inDepthTestFunc, bool inDepthWriteEnabled)
	{
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferFunction = GetXNACompareFunc(inDepthTestFunc);
		depthStencilState.DepthBufferEnable = inDepthWriteEnabled;
		depthStencilState.DepthBufferWriteEnable = inDepthWriteEnabled;
		mStateMgr.SetDepthStencilState(depthStencilState);
	}

	public override void SetAlphaTest(Graphics3D.ECompareFunc inAlphaTestFunc, int inRefAlpha)
	{
	}

	public override void SetColorWriteState(int inWriteRedEnabled, int inWriteGreenEnabled, int inWriteBlueEnabled, int inWriteAlphaEnabled)
	{
	}

	public override void SetWireframe(int inWireframe)
	{
	}

	public override void SetBlend(Graphics3D.EBlendMode inSrcBlend, Graphics3D.EBlendMode inDestBlend)
	{
		mStateMgr.SetBlendOverride(inSrcBlend, inDestBlend);
	}

	public override void SetBackfaceCulling(int inCullClockwise, int inCullCounterClockwise)
	{
	}

	public override void SetLightingEnabled(int inLightingEnabled)
	{
	}

	public override void SetLightEnabled(int inLightIndex, int inEnabled)
	{
	}

	public void DoCommitAllRenderState()
	{
		mBasicEffect.Projection = mStateMgr.mXNAProjectionMatrix;
		mBasicEffect.View = mStateMgr.mXNAViewMatrix;
		mBasicEffect.World = mStateMgr.mXNAWorldMatrix;
		mBasicEffect.VertexColorEnabled = true;
		if (mStateMgr.mXNATextureSlots != null)
		{
			mBasicEffect.Texture = mStateMgr.mXNATextureSlots;
			mBasicEffect.TextureEnabled = true;
		}
		else
		{
			mBasicEffect.TextureEnabled = false;
		}
		mBasicEffect.GraphicsDevice.RasterizerState = mStateMgr.mXNARasterizerState;
		mBasicEffect.GraphicsDevice.BlendState = mStateMgr.mXNABlendState;
		mBasicEffect.GraphicsDevice.DepthStencilState = mStateMgr.mXNADepthStencilState;
		mBasicEffect.GraphicsDevice.SamplerStates[0] = mStateMgr.mXNASamplerStateSlots;
	}

	public void DoCommitLastAllRenderState()
	{
		if (mStateMgr.mProjectMatrixDirty)
		{
			mBasicEffect.Projection = mStateMgr.mXNALastProjectionMatrix;
		}
		else
		{
			mBasicEffect.Projection = mStateMgr.mXNAProjectionMatrix;
		}
		mBasicEffect.View = mStateMgr.mXNAViewMatrix;
		mBasicEffect.World = mStateMgr.mXNALastWorldMatrix;
		mBasicEffect.VertexColorEnabled = true;
		if (mStateMgr.mLastXNATextureSlots != null)
		{
			mBasicEffect.Texture = mStateMgr.mLastXNATextureSlots;
			mBasicEffect.TextureEnabled = true;
		}
		else
		{
			mBasicEffect.TextureEnabled = false;
		}
		mBasicEffect.GraphicsDevice.RasterizerState = mStateMgr.mXNARasterizerState;
		mBasicEffect.GraphicsDevice.BlendState = mStateMgr.mXNALastBlendState;
		if (mStateMgr.mStencilStateDirty)
		{
			mBasicEffect.GraphicsDevice.DepthStencilState = mStateMgr.mXNALastStencilState;
		}
		else
		{
			mBasicEffect.GraphicsDevice.DepthStencilState = mStateMgr.mXNADepthStencilState;
		}
		mBasicEffect.GraphicsDevice.SamplerStates[0] = mStateMgr.mXNALastSamplerStateSlots;
	}

	public override void ClearRect(Rect theRect)
	{
	}

	public override void FillRect(Rect theRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
	{
		if (PreDraw())
		{
			SetupDrawMode(theDrawMode);
			float num = (float)theRect.mX + mPixelOffset;
			float num2 = (float)theRect.mY + mPixelOffset;
			float num3 = theRect.mWidth;
			float num4 = theRect.mHeight;
			float z = 0f;
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			mTmpVPCBuffer[0].Position = new Vector3(num, num2, z);
			mTmpVPCBuffer[0].Color = color;
			mTmpVPCBuffer[1].Position = new Vector3(num, num2 + num4, z);
			mTmpVPCBuffer[1].Color = color;
			mTmpVPCBuffer[2].Position = new Vector3(num + num3, num2, z);
			mTmpVPCBuffer[2].Color = color;
			mTmpVPCBuffer[3].Position = new Vector3(num + num3, num2 + num4, z);
			mTmpVPCBuffer[3].Color = color;
			SetTextureDirect(0, null);
			mStateMgr.SetWorldTransform(Matrix.Identity);
			DrawPrimitiveInternal(5, 2, mTmpVPCBuffer, 32uL, mDefaultVertexFVF, inDoCommit: true, Matrix.Identity);
		}
	}

	public override void FillScanLinesWithCoverage(Span theSpans, int theSpanCount, SexyFramework.Graphics.Color theColor, int theDrawMode, string theCoverage, int theCoverX, int theCoverY, int theCoverWidth, int theCoverHeight)
	{
	}

	public override void FillPoly(SexyFramework.Misc.Point[] theVertices, int theNumVertices, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, int tx, int ty)
	{
		if (theNumVertices < 3 || !PreDraw())
		{
			return;
		}
		SetupDrawMode(theDrawMode);
		Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
		float z = 0f;
		VertexPositionColorTexture[] array = new VertexPositionColorTexture[theNumVertices];
		for (int i = 0; i < theNumVertices; i++)
		{
			array[i].Position = new Vector3((float)theVertices[i].mX + (float)tx, (float)theVertices[i].mY + (float)ty, z);
			array[i].Color = color;
			if (mTransformStack.Count != 0)
			{
				SexyVector2 sexyVector = new SexyVector2(array[i].Position.X, array[i].Position.Y);
				sexyVector = mTransformStack.Peek() * sexyVector;
				array[i].Position.X = sexyVector.x;
				array[i].Position.Y = sexyVector.y;
			}
		}
		DrawPolyClipped(theClipRect, array);
	}

	public void DrawPolyClipped(Rect theClipRect, VertexPositionColorTexture[] theList)
	{
		List<VertexPositionColorTexture> list = new List<VertexPositionColorTexture>();
		List<VertexPositionColorTexture> list2 = new List<VertexPositionColorTexture>(theList);
		int mX = theClipRect.mX;
		int num = mX + theClipRect.mWidth;
		int mY = theClipRect.mY;
		int num2 = mY + theClipRect.mHeight;
		ClipPoints(0, mX, ClipperType.Clipper_Less, list2, list);
		list2.Clear();
		ClipPoints(1, mY, ClipperType.Clipper_Less, list, list2);
		list.Clear();
		ClipPoints(0, num, ClipperType.Clipper_GreaterEqual, list2, list);
		list2.Clear();
		ClipPoints(1, num2, ClipperType.Clipper_GreaterEqual, list, list2);
		CheckBatchAndCommit();
		if (list2.Count >= 3)
		{
			BufferedDrawPrimitive(6, list2.Count - 2, list2.ToArray(), (int)mDefaultVertexSize, mDefaultVertexFVF, Matrix.Identity);
		}
	}

	public void ClipPoint(int index, float clipValue, ClipperType type, VertexPositionColorTexture vertex1, VertexPositionColorTexture vertex2, List<VertexPositionColorTexture> outList)
	{
		float vertexValue = GetVertexValue(index, vertex1);
		float vertexValue2 = GetVertexValue(index, vertex2);
		switch (type)
		{
		case ClipperType.Clipper_Less:
			if (vertexValue >= clipValue)
			{
				if (vertexValue2 >= clipValue)
				{
					outList.Add(vertex2);
					break;
				}
				float t3 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
				outList.Add(Interpolate(vertex1, vertex2, t3));
			}
			else if (vertexValue2 >= clipValue)
			{
				float t4 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
				outList.Add(Interpolate(vertex1, vertex2, t4));
				outList.Add(vertex2);
			}
			break;
		case ClipperType.Clipper_GreaterEqual:
			if (vertexValue < clipValue)
			{
				if (vertexValue2 < clipValue)
				{
					outList.Add(vertex2);
					break;
				}
				float t = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
				outList.Add(Interpolate(vertex1, vertex2, t));
			}
			else if (vertexValue2 < clipValue)
			{
				float t2 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
				outList.Add(Interpolate(vertex1, vertex2, t2));
				outList.Add(vertex2);
			}
			break;
		case ClipperType.Clipper_Greater:
		case ClipperType.Clipper_Equal:
			break;
		}
	}

	public void ClipPoints(int index, float clipValue, ClipperType type, List<VertexPositionColorTexture> inList, List<VertexPositionColorTexture> outList)
	{
		if (inList.Count >= 2)
		{
			ClipPoint(index, clipValue, type, inList[inList.Count - 1], inList[0], outList);
			for (int i = 0; i < inList.Count - 1; i++)
			{
				ClipPoint(index, clipValue, type, inList[i], inList[i + 1], outList);
			}
		}
	}

	public float GetVertexValue(int index, VertexPositionColorTexture vertex)
	{
		return index switch
		{
			0 => vertex.Position.X, 
			1 => vertex.Position.Y, 
			2 => vertex.Position.Z, 
			_ => 0f, 
		};
	}

	private VertexPositionColorTexture Interpolate(VertexPositionColorTexture v1, VertexPositionColorTexture v2, float t)
	{
		VertexPositionColorTexture result = v1;
		result.Position.X = v1.Position.X + t * (v2.Position.X - v1.Position.X);
		result.Position.Y = v1.Position.Y + t * (v2.Position.Y - v1.Position.Y);
		result.TextureCoordinate.X = v1.TextureCoordinate.X + t * (v2.TextureCoordinate.X - v1.TextureCoordinate.X);
		result.TextureCoordinate.Y = v1.TextureCoordinate.Y + t * (v2.TextureCoordinate.Y - v1.TextureCoordinate.Y);
		if (v1.Color != v2.Color)
		{
			Vector4 vector = Vector4.Lerp(v1.Color.ToVector4(), v2.Color.ToVector4(), t);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(vector);
			result.Color = color;
		}
		return result;
	}

	public override void DrawLine(double theStartX, double theStartY, double theEndX, double theEndY, SexyFramework.Graphics.Color theColor, int theDrawMode, bool antiAlias)
	{
		if (PreDraw())
		{
			SetupDrawMode(theDrawMode);
			float x = (float)theStartX;
			float y = (float)theStartY;
			float x2 = (float)theEndX;
			float y2 = (float)theEndY;
			float z = 0f;
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			VertexPositionColor[] inVertData = new VertexPositionColor[2]
			{
				new VertexPositionColor(new Vector3(x, y, z), color),
				new VertexPositionColor(new Vector3(x2, y2, z), color)
			};
			SetTextureDirect(0, null);
			mStateMgr.SetWorldTransform(Matrix.Identity);
			DoCommitAllRenderState();
			DrawPrimitiveInternal(3, 1, inVertData, 32uL, mDefaultVertexFVF, inDoCommit: false, Matrix.Identity);
		}
	}

	public override void Blt(Image theImage, int theX, int theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
	{
		BltNoClipF(theImage, theX, theY, theSrcRect, theColor, theDrawMode, linearFilter: false);
	}

	public override void BltF(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
	{
		FRect theTRect = new FRect(theClipRect.mX, theClipRect.mY, theClipRect.mWidth, theClipRect.mHeight);
		FRect fRect = new FRect(theX, theY, theSrcRect.mWidth, theSrcRect.mHeight);
		FRect fRect2 = fRect.Intersection(theTRect);
		if (fRect2.mWidth != fRect.mWidth || fRect2.mHeight != fRect.mHeight)
		{
			if (fRect2.mWidth != 0f && fRect2.mHeight != 0f)
			{
				BltClipF(theImage, theX, theY, theSrcRect, theClipRect, theColor, theDrawMode);
			}
		}
		else
		{
			BltNoClipF(theImage, theX, theY, theSrcRect, theColor, theDrawMode, linearFilter: true);
		}
	}

	public override void BltRotated(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, double theRot, float theRotCenterX, float theRotCenterY)
	{
		mTransform.Reset();
		mTransform.Translate(0f - theRotCenterX, 0f - theRotCenterY);
		mTransform.RotateRad((float)theRot);
		mTransform.Translate(theX + theRotCenterX, theY + theRotCenterY);
		BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), linearFilter: true, 0f, 0f, center: false);
	}

	private void BltTransformed(Image theImage, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, SexyMatrix3 theTransform, bool linearFilter, float theX, float theY, bool center)
	{
		if (!PreDraw())
		{
			return;
		}
		SexyTransform2D sexyTransform2D = (SexyTransform2D)(object)theTransform;
		if (mTransformStack.Count != 0)
		{
			if (theX != 0f || theY != 0f)
			{
				SexyTransform2D sexyTransform2D2 = new SexyTransform2D(init: false);
				if (center)
				{
					sexyTransform2D2.Translate((float)(-theSrcRect.mWidth) / 2f, (float)(-theSrcRect.mHeight) / 2f);
				}
				sexyTransform2D2 = sexyTransform2D * sexyTransform2D2;
				sexyTransform2D2.Translate(theX, theY);
				sexyTransform2D2 = mTransformStack.Peek() * sexyTransform2D2;
				BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, sexyTransform2D2, linearFilter, theX, theY, center);
			}
			else
			{
				SexyTransform2D theTransform2 = mTransformStack.Peek() * sexyTransform2D;
				BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theTransform2, linearFilter, theX, theY, center);
			}
		}
		else
		{
			BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, sexyTransform2D, linearFilter, theX, theY, center);
		}
	}

	public override void BltMatrix(Image theImage, float x, float y, SexyMatrix3 theMatrix, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, bool blend)
	{
		BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theMatrix, blend, x, y, center: true);
	}

	public override void BltTriangles(Image theImage, SexyVertex2D[,] theVertices, int theNumTriangles, SexyFramework.Graphics.Color theColor, int theDrawMode, float tx, float ty, bool blend, Rect theClipRect)
	{
		BltTrianglesHelper(theImage, theVertices, theNumTriangles, theColor, theDrawMode, tx, ty, blend, theClipRect);
	}

	public void BltTrianglesHelper(Image theImage, SexyVertex2D[,] theVertices, int theNumTriangles, SexyFramework.Graphics.Color theColor, int theDrawMode, float tx, float ty, bool blend, Rect theClipRect)
	{
		theImage = SetupAtlasState(0, theImage);
		MemoryImage inImage = theImage as MemoryImage;
		if (!CreateImageRenderData(ref inImage))
		{
			return;
		}
		SetupDrawMode(theDrawMode);
		XNATextureData xNATextureData = (XNATextureData)inImage.GetRenderData();
		if (!((double)xNATextureData.mMaxTotalU <= 1.0) || !((double)xNATextureData.mMaxTotalV <= 1.0))
		{
			return;
		}
		SetTextureDirect(0, xNATextureData.mTextures[0].mTexture);
		float z = 0f;
		bool flag = mTransformStack.Count != 0;
		bool flag2 = theClipRect.mX != 0 || theClipRect.mY != 0 || theClipRect.mWidth != mScreenWidth || theClipRect.mHeight != mScreenHeight;
		CheckBatchAndCommit();
		if (flag)
		{
			SexyMatrix3 sexyMatrix = mTransformStack.Peek();
			for (int i = 0; i < theNumTriangles; i++)
			{
				if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				SexyVector2[] array = new SexyVector2[3];
				array[0].x = theVertices[i, 0].x + tx;
				array[0].y = theVertices[i, 0].y + ty;
				array[1].x = theVertices[i, 1].x + tx;
				array[1].y = theVertices[i, 1].y + ty;
				array[2].x = theVertices[i, 2].x + tx;
				array[2].y = theVertices[i, 2].y + ty;
				array[0].x = array[0].x * sexyMatrix.m00 + array[0].y * sexyMatrix.m01 + sexyMatrix.m02;
				array[0].y = array[0].x * sexyMatrix.m10 + array[0].y * sexyMatrix.m11 + sexyMatrix.m12;
				array[1].x = array[1].x * sexyMatrix.m00 + array[1].y * sexyMatrix.m01 + sexyMatrix.m02;
				array[1].y = array[1].x * sexyMatrix.m10 + array[1].y * sexyMatrix.m11 + sexyMatrix.m12;
				array[2].x = array[2].x * sexyMatrix.m00 + array[2].y * sexyMatrix.m01 + sexyMatrix.m02;
				array[2].y = array[2].x * sexyMatrix.m10 + array[2].y * sexyMatrix.m11 + sexyMatrix.m12;
				ref VertexPositionColorTexture reference = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference = new VertexPositionColorTexture(new Vector3(array[0].x, array[0].y, z), (theVertices[i, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[i, 0].color) : GetXNAColor(theColor), new Vector2(theVertices[i, 0].u * xNATextureData.mMaxTotalU, theVertices[i, 0].v * xNATextureData.mMaxTotalV));
				ref VertexPositionColorTexture reference2 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference2 = new VertexPositionColorTexture(new Vector3(array[1].x, array[1].y, z), (theVertices[i, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[i, 1].color) : GetXNAColor(theColor), new Vector2(theVertices[i, 1].u * xNATextureData.mMaxTotalU, theVertices[i, 1].v * xNATextureData.mMaxTotalV));
				ref VertexPositionColorTexture reference3 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference3 = new VertexPositionColorTexture(new Vector3(array[2].x, array[2].y, z), (theVertices[i, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[i, 2].color) : GetXNAColor(theColor), new Vector2(theVertices[i, 2].u * xNATextureData.mMaxTotalU, theVertices[i, 2].v * xNATextureData.mMaxTotalV));
				new SexyVector2(mBatchedTriangleBuffer[mBatchedTriangleIndex - 3].TextureCoordinate.X, mBatchedTriangleBuffer[mBatchedTriangleIndex - 3].TextureCoordinate.Y);
				new SexyVector2(mBatchedTriangleBuffer[mBatchedTriangleIndex - 2].TextureCoordinate.X, mBatchedTriangleBuffer[mBatchedTriangleIndex - 2].TextureCoordinate.Y);
				new SexyVector2(mBatchedTriangleBuffer[mBatchedTriangleIndex - 1].TextureCoordinate.X, mBatchedTriangleBuffer[mBatchedTriangleIndex - 1].TextureCoordinate.Y);
				AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, mBatchedTriangleIndex - 3, 3, 0u, 32, 0);
				if (!SUPPORT_HW_CLIP && flag2)
				{
					VertexPositionColorTexture[] array2 = new VertexPositionColorTexture[3];
					for (int j = 0; j < 3; j++)
					{
						ref VertexPositionColorTexture reference4 = ref array2[j];
						reference4 = mBatchedTriangleBuffer[mBatchedTriangleIndex - (3 - j)];
					}
					mBatchedTriangleIndex -= 3;
					DrawPolyClipped(theClipRect, array2);
				}
			}
			return;
		}
		if (!SUPPORT_HW_CLIP && flag2)
		{
			for (int k = 0; k < theNumTriangles; k++)
			{
				if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				ref VertexPositionColorTexture reference5 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference5 = new VertexPositionColorTexture(new Vector3(theVertices[k, 0].x, theVertices[k, 0].y, z), (theVertices[k, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 0].color) : GetXNAColor(theColor), new Vector2(theVertices[k, 0].u * xNATextureData.mMaxTotalU, theVertices[k, 0].v * xNATextureData.mMaxTotalV));
				ref VertexPositionColorTexture reference6 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference6 = new VertexPositionColorTexture(new Vector3(theVertices[k, 1].x, theVertices[k, 1].y, z), (theVertices[k, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 1].color) : GetXNAColor(theColor), new Vector2(theVertices[k, 1].u * xNATextureData.mMaxTotalU, theVertices[k, 1].v * xNATextureData.mMaxTotalV));
				ref VertexPositionColorTexture reference7 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference7 = new VertexPositionColorTexture(new Vector3(theVertices[k, 2].x, theVertices[k, 2].y, z), (theVertices[k, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 2].color) : GetXNAColor(theColor), new Vector2(theVertices[k, 2].u * xNATextureData.mMaxTotalU, theVertices[k, 2].v * xNATextureData.mMaxTotalV));
				AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, mBatchedTriangleIndex - 3, 3, 0u, 32, 0);
				if (!SUPPORT_HW_CLIP && flag2)
				{
					VertexPositionColorTexture[] array3 = new VertexPositionColorTexture[3];
					for (int l = 0; l < 3; l++)
					{
						ref VertexPositionColorTexture reference8 = ref array3[l];
						reference8 = mBatchedTriangleBuffer[mBatchedTriangleIndex - (3 - l)];
					}
					mBatchedTriangleIndex -= 3;
					DrawPolyClipped(theClipRect, array3);
				}
			}
		}
		else
		{
			int num = 0;
			while (num < theNumTriangles)
			{
				if (mBatchedTriangleIndex >= mBatchedTriangleSize)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				int inStartIndex = mBatchedTriangleIndex;
				int num2 = 0;
				int num3 = Math.Min(mBatchedTriangleSize - mBatchedTriangleIndex, theNumTriangles - num);
				while (num2 < num3)
				{
					ref VertexPositionColorTexture reference9 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
					reference9 = new VertexPositionColorTexture(new Vector3(theVertices[num, 0].x, theVertices[num, 0].y, z), (theVertices[num, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num, 0].color) : GetXNAColor(theColor), new Vector2(theVertices[num, 0].u * xNATextureData.mMaxTotalU, theVertices[num, 0].v * xNATextureData.mMaxTotalV));
					ref VertexPositionColorTexture reference10 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
					reference10 = new VertexPositionColorTexture(new Vector3(theVertices[num, 1].x, theVertices[num, 1].y, z), (theVertices[num, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num, 1].color) : GetXNAColor(theColor), new Vector2(theVertices[num, 1].u * xNATextureData.mMaxTotalU, theVertices[num, 1].v * xNATextureData.mMaxTotalV));
					ref VertexPositionColorTexture reference11 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
					reference11 = new VertexPositionColorTexture(new Vector3(theVertices[num, 2].x, theVertices[num, 2].y, z), (theVertices[num, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num, 2].color) : GetXNAColor(theColor), new Vector2(theVertices[num, 2].u * xNATextureData.mMaxTotalU, theVertices[num, 2].v * xNATextureData.mMaxTotalV));
					num2 += 3;
					num++;
				}
				AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, inStartIndex, num2, 0u, 32, 0);
			}
		}
		if (mBatchedTriangleIndex > 0 && (mRenderModeFlags & 1) != 0)
		{
			DrawPrimitiveInternal(4, mBatchedTriangleIndex / 3, mBatchedTriangleBuffer, 32uL, 32uL, inDoCommit: false, Matrix.Identity);
			mBatchedTriangleIndex = 0;
		}
	}

	private void CheckBatchAndCommit()
	{
		if (mSceneBegun && mBatchedTriangleIndex >= 0 && (mStateMgr.mStateDirty || mStateMgr.mTextureStateDirty || mStateMgr.mProjectMatrixDirty || mStateMgr.mStencilStateDirty))
		{
			if (mBatchedTriangleIndex > 0)
			{
				DoCommitLastAllRenderState();
				FlushBufferedTriangles();
			}
			mStateMgr.mStateDirty = false;
			mStateMgr.mTextureStateDirty = false;
			mStateMgr.mProjectMatrixDirty = false;
			mStateMgr.mStencilStateDirty = false;
		}
	}

	private void FlushBufferedTriangles()
	{
		if (mSceneBegun && mBatchedTriangleIndex > 0)
		{
			int inPrimCount = mBatchedTriangleIndex / 3;
			DrawPrimitiveInternal(4, inPrimCount, mBatchedTriangleBuffer, 32uL, mDefaultVertexFVF, inDoCommit: false, Matrix.Identity);
			mBatchedTriangleIndex = 0;
			mBatchedIndexIndex = 0;
		}
	}

	public override void BltMirror(Image theImage, int theX, int theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
	{
		mTransform.Reset();
		mTransform.Translate(0f - (float)theSrcRect.mWidth, 0f);
		mTransform.Scale(-1f, 1f);
		mTransform.Translate(theX, theY);
		BltTransformed(theImage, Rect.INVALIDATE_RECT, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), linearFilter: false, 0f, 0f, center: false);
	}

	public override void BltStretched(Image theImage, Rect theDestRect, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool fastStretch, bool mirror)
	{
		float num = (float)theDestRect.mWidth / (float)theSrcRect.mWidth;
		float sy = (float)theDestRect.mHeight / (float)theSrcRect.mHeight;
		mTransform.Reset();
		if (mirror)
		{
			mTransform.Translate(0f - (float)theSrcRect.mWidth, 0f);
			mTransform.Scale(0f - num, sy);
		}
		else
		{
			mTransform.Scale(num, sy);
		}
		mTransform.Translate(theDestRect.mX, theDestRect.mY);
		BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), !fastStretch, 0f, 0f, center: false);
	}

	public override void SetGlobalAmbient(SexyFramework.Graphics.Color inColor)
	{
	}

	public void Init(int width, int height)
	{
		mDevice.IsFullScreen = false;
		int num = (mDevice.PreferredBackBufferWidth = PhysicalBackBufferWidth);
		mScreenWidth = num;
		int num3 = (mDevice.PreferredBackBufferHeight = PhysicalBackBufferHeight);
		mScreenHeight = num3;
		mWidth = width;
		mHeight = height;
		mDevice.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
		mDevice.ApplyChanges();
		UpdatePresentationRect();
		mTmpVPCTBuffer = new VertexPositionColorTexture[4];
		mTmpVPCBuffer = new VertexPositionColor[4];
		mBasicEffect = new BasicEffect(mDevice.GraphicsDevice);
		mSpriteBatch = new SpriteBatch(mDevice.GraphicsDevice);
		mAlphaTestEffect = new AlphaTestEffect(mDevice.GraphicsDevice);
		mAdditiveState.AlphaDestinationBlend = Blend.One;
		mAdditiveState.ColorDestinationBlend = Blend.One;
		mAdditiveState.AlphaSourceBlend = Blend.SourceAlpha;
		mAdditiveState.ColorSourceBlend = Blend.SourceAlpha;
		mNormalState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		mNormalState.ColorDestinationBlend = Blend.InverseSourceAlpha;
		mNormalState.AlphaSourceBlend = Blend.SourceAlpha;
		mNormalState.ColorSourceBlend = Blend.SourceAlpha;
		SetSamplerState(0, 0);
		SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);
		SetDepthState(Graphics3D.ECompareFunc.COMPARE_NEVER, inDepthWriteEnabled: false);
		SetRasterizerState(0, 0);
		SetDefaultState(null, isInScene: false);
		mStateMgr.mStateDirty = false;
		mCurDrawMode = 0;
	}

	public void SetDefaultState(Image theImage, bool isInScene)
	{
		int num = mWidth;
		int num2 = mHeight;
		if (theImage != null)
		{
			num = theImage.mWidth;
			num2 = theImage.mHeight;
		}
		if (theImage == null)
		{
			SetViewport(mRenderRect.X, mRenderRect.Y, mRenderRect.Width, mRenderRect.Height, 0f, 1f);
		}
		else
		{
			SetViewport(0, 0, theImage.mWidth, theImage.mHeight, 0f, 1f);
		}
		mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, num, num2, 0f, -1000f, 1000f));
		mStateMgr.SetViewTransform(Matrix.CreateLookAt(new Vector3(0f, 0f, 300f), Vector3.Zero, Vector3.Up));
		mStateMgr.SetWorldTransform(Matrix.Identity);
	}

	public override void SetViewport(int theX, int theY, int theWidth, int theHeight, float theMinZ, float theMaxZ)
	{
		mStateMgr.SetViewport(theX, theY, theWidth, theHeight, theMinZ, theMaxZ);
		mDevice.GraphicsDevice.Viewport = mStateMgr.mXNAViewPort;
	}

	public void SetTextureDirect(int theStage, Texture2D theTexture)
	{
		mStateMgr.SetTexture(theTexture);
	}

	public void SetRenderState(SEXY3DRSS theRenderState, uint theValue)
	{
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = false;
		depthStencilState.DepthBufferWriteEnable = false;
		mDevice.GraphicsDevice.DepthStencilState = depthStencilState;
		RasterizerState rasterizerState = new RasterizerState();
		rasterizerState.CullMode = CullMode.None;
		mDevice.GraphicsDevice.RasterizerState = rasterizerState;
		mDevice.GraphicsDevice.BlendState = BlendState.AlphaBlend;
	}

	public void SetSamplerState(int theSampler, int theValue)
	{
		mStateMgr.SetSamplerState(SamplerState.LinearClamp);
	}

	public void SetRasterizerState(int fillMode, int cullMode)
	{
		RasterizerState rasterizerState = new RasterizerState();
		rasterizerState.FillMode = (FillMode)fillMode;
		rasterizerState.CullMode = (CullMode)cullMode;
		mStateMgr.SetRasterizerState(rasterizerState);
	}

	public override bool SetTexture(int inTextureIndex, Image inImage)
	{
		if (inImage == null)
		{
			mImage = null;
			SetTextureDirect(inTextureIndex, null);
			return true;
		}
		mImage = inImage;
		inImage = SetupAtlasState(inTextureIndex, inImage);
		MemoryImage inImage2 = inImage.AsMemoryImage();
		if (inImage2 == null)
		{
			return false;
		}
		if (!CreateImageRenderData(ref inImage2))
		{
			return false;
		}
		XNATextureData xNATextureData = (XNATextureData)inImage2.GetRenderData();
		if ((xNATextureData.mImageFlags & 0x20) == 0 && (xNATextureData.mImageFlags & 0x40) == 0)
		{
			SetTextureDirect(inTextureIndex, xNATextureData.mTextures[0].mTexture);
		}
		return true;
	}

	private void BltClipF(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
	{
		SexyTransform2D sexyTransform2D = new SexyTransform2D(init: false);
		sexyTransform2D.Translate(theX, theY);
		BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, sexyTransform2D, linearFilter: true, 0f, 0f, center: false);
	}

	public void BltNoClipF(Image theImage, float theX, float theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool linearFilter)
	{
		if (mTransformStack.Count != 0)
		{
			BltClipF(theImage, theX, theY, theSrcRect, Rect.INVALIDATE_RECT, theColor, theDrawMode);
		}
		else if (PreDraw())
		{
			BltHelper(theImage, theX, theY, theSrcRect, theColor, theDrawMode, linearFilter);
		}
	}

	public void BltTransformHelper(Image theImage, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, SexyTransform2D theTransform, bool linearFilter, float theX, float theY, bool center)
	{
		Image image = theImage;
		image.InitAtalasState();
		theImage = SetupAtlasState(0, theImage);
		int mX = theSrcRect.mX;
		int mY = theSrcRect.mY;
		int num = mX + theSrcRect.mWidth;
		int num2 = mY + theSrcRect.mHeight;
		int num3 = 0;
		int num4 = 0;
		float u = 0f;
		float v = 0f;
		float u2 = 0f;
		float v2 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		MemoryImage inImage = theImage as MemoryImage;
		if (!CreateImageRenderData(ref inImage))
		{
			return;
		}
		SetupDrawMode(theDrawMode);
		bool flag = false;
		if (theDrawMode == 0 && !inImage.mHasAlpha && theColor.mAlpha >= 255 && image.mWidth * image.mHeight > 40000)
		{
			SetBlend(Graphics3D.EBlendMode.BLEND_ONE, Graphics3D.EBlendMode.BLEND_ZERO);
			flag = true;
		}
		XNATextureData xNATextureData = (XNATextureData)inImage.GetRenderData();
		if (center)
		{
			num5 = (float)(-theSrcRect.mWidth) / 2f;
			num6 = (float)(-theSrcRect.mHeight) / 2f;
		}
		int num7 = mY;
		float num8 = num6;
		if (mX >= num || mY >= num2)
		{
			return;
		}
		theTransform.Translate(theX, theY);
		float z = 0f;
		Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
		int num9 = mX;
		float num10 = num5;
		num3 = num - num9;
		num4 = num2 - num7;
		Texture2D texture = xNATextureData.GetTexture(image as MemoryImage, num9, num7, ref num3, ref num4, ref u, ref v, ref u2, ref v2);
		SetTextureDirect(0, texture);
		float num11 = num10;
		float num12 = num8;
		mTmpVPCTBuffer[0].Position.X = num11;
		mTmpVPCTBuffer[0].Position.Y = num12;
		mTmpVPCTBuffer[0].Position.Z = z;
		mTmpVPCTBuffer[0].Color = color;
		mTmpVPCTBuffer[0].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v;
		mTmpVPCTBuffer[1].Position.X = num11;
		mTmpVPCTBuffer[1].Position.Y = num12 + (float)num4;
		mTmpVPCTBuffer[1].Position.Z = z;
		mTmpVPCTBuffer[1].Color = color;
		mTmpVPCTBuffer[1].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v2;
		mTmpVPCTBuffer[2].Position.X = num11 + (float)num3;
		mTmpVPCTBuffer[2].Position.Y = num12;
		mTmpVPCTBuffer[2].Position.Z = z;
		mTmpVPCTBuffer[2].Color = color;
		mTmpVPCTBuffer[2].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v;
		mTmpVPCTBuffer[3].Position.X = num11 + (float)num3;
		mTmpVPCTBuffer[3].Position.Y = num12 + (float)num4;
		mTmpVPCTBuffer[3].Position.Z = z;
		mTmpVPCTBuffer[3].Color = color;
		mTmpVPCTBuffer[3].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v2;
		Matrix matrix = theTransform.mMatrix;
		for (int i = 0; i < 4; i++)
		{
			Vector3.Transform(ref mTmpVPCTBuffer[i].Position, ref matrix, out mTmpVPCTBuffer[i].Position);
		}
		Rect rect = theClipRect;
		bool flag2 = false;
		if (rect != Rect.INVALIDATE_RECT && (rect.mX != 0 || rect.mY != 0 || rect.mWidth != mWidth || rect.mHeight != mHeight))
		{
			SexyVector2 sexyVector = new SexyVector2(rect.mX, rect.mY);
			SexyVector2 sexyVector2 = new SexyVector2(rect.mX + rect.mWidth, rect.mY + rect.mHeight);
			for (int j = 0; j < 4; j++)
			{
				if (mTmpVPCTBuffer[j].Position.X < sexyVector.x || mTmpVPCTBuffer[j].Position.X >= sexyVector2.x || mTmpVPCTBuffer[j].Position.Y < sexyVector.y || mTmpVPCTBuffer[j].Position.Y >= sexyVector2.y)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (flag2)
		{
			VertexPositionColorTexture vertexPositionColorTexture = mTmpVPCTBuffer[2];
			ref VertexPositionColorTexture reference = ref mTmpVPCTBuffer[2];
			reference = mTmpVPCTBuffer[3];
			mTmpVPCTBuffer[3] = vertexPositionColorTexture;
			DrawPolyClipped(rect, mTmpVPCTBuffer);
		}
		else
		{
			BufferedDrawPrimitive(5, 2, mTmpVPCTBuffer, 32, mDefaultVertexFVF, Matrix.Identity);
		}
		if (flag)
		{
			SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);
		}
	}

	public override void DrawSprite(Image theImage, SexyFramework.Graphics.Color theColor, int theDrawMode, SexyTransform2D theTransform, Rect theSrcRect, bool center)
	{
		Image image = theImage;
		image.InitAtalasState();
		theImage = SetupAtlasState(0, theImage);
		MemoryImage inImage = theImage as MemoryImage;
		if (CreateImageRenderData(ref inImage))
		{
			SetupDrawMode(theDrawMode);
			XNATextureData xNATextureData = (XNATextureData)inImage.GetRenderData();
			new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			Rectangle value = new Rectangle(image.mAtlasStartX + theSrcRect.mX, image.mAtlasStartY + theSrcRect.mY, theSrcRect.mWidth, theSrcRect.mHeight);
			Texture2D texture = xNATextureData.mTextures[0].mTexture;
			Vector2 position = new Vector2(theTransform.mMatrix.M41, theTransform.mMatrix.M42);
			Vector2 origin = ((!center) ? new Vector2(0f, 0f) : new Vector2((float)value.Width / 2f, (float)value.Height / 2f));
			float scale = (float)Math.Sqrt(theTransform.mMatrix.M11 * theTransform.mMatrix.M11 + theTransform.mMatrix.M12 * theTransform.mMatrix.M12) * (float)Math.Sqrt(theTransform.mMatrix.M21 * theTransform.mMatrix.M21 + theTransform.mMatrix.M22 * theTransform.mMatrix.M22);
			float rotation = (float)Math.Asin(theTransform.mMatrix.M21);
			mSpriteBatch.Draw(texture, position, value, Microsoft.Xna.Framework.Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
		}
	}

	public override void BeginSprite()
	{
		FlushBufferedTriangles();
		mSpriteBatch.Begin();
	}

	public override void EndSprite()
	{
		mSpriteBatch.End();
	}

	public void BltHelper(Image theImage, float theX, float theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool linearFilter)
	{
		Image image = theImage;
		image.InitAtalasState();
		theImage = SetupAtlasState(0, theImage);
		MemoryImage inImage = theImage as MemoryImage;
		if (CreateImageRenderData(ref inImage))
		{
			SetupDrawMode(theDrawMode);
			XNATextureData xNATextureData = (XNATextureData)inImage.GetRenderData();
			int mX = theSrcRect.mX;
			int mY = theSrcRect.mY;
			int num = mX + theSrcRect.mWidth;
			int num2 = mY + theSrcRect.mHeight;
			int num3 = 0;
			int num4 = 0;
			float u = 0f;
			float v = 0f;
			float u2 = 0f;
			float v2 = 0f;
			int num5 = mY;
			if (mX < num && mY < num2)
			{
				float z = 0f;
				Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
				int num6 = mX;
				num3 = num - num6;
				num4 = num2 - num5;
				Texture2D texture = xNATextureData.GetTexture((MemoryImage)image, num6, num5, ref num3, ref num4, ref u, ref v, ref u2, ref v2);
				float num7 = theY;
				mTmpVPCTBuffer[0].Position.X = theX;
				mTmpVPCTBuffer[0].Position.Y = num7;
				mTmpVPCTBuffer[0].Position.Z = z;
				mTmpVPCTBuffer[0].Color = color;
				mTmpVPCTBuffer[0].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v;
				mTmpVPCTBuffer[1].Position.X = theX;
				mTmpVPCTBuffer[1].Position.Y = num7 + (float)num4;
				mTmpVPCTBuffer[1].Position.Z = z;
				mTmpVPCTBuffer[1].Color = color;
				mTmpVPCTBuffer[1].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v2;
				mTmpVPCTBuffer[2].Position.X = theX + (float)num3;
				mTmpVPCTBuffer[2].Position.Y = num7;
				mTmpVPCTBuffer[2].Position.Z = z;
				mTmpVPCTBuffer[2].Color = color;
				mTmpVPCTBuffer[2].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v;
				mTmpVPCTBuffer[3].Position.X = theX + (float)num3;
				mTmpVPCTBuffer[3].Position.Y = num7 + (float)num4;
				mTmpVPCTBuffer[3].Position.Z = z;
				mTmpVPCTBuffer[3].Color = color;
				mTmpVPCTBuffer[3].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v2;
				SetTextureDirect(0, texture);
				BufferedDrawPrimitive(5, 2, mTmpVPCTBuffer, 32, mDefaultVertexFVF, Matrix.Identity);
			}
		}
	}

	public bool PreDraw()
	{
		if (!mSceneBegun)
		{
			mSceneBegun = true;
			RenderStateManager.Context context = mStateMgr.GetContext();
			mStateMgr.SetContext(null);
			mStateMgr.RevertState();
			mStateMgr.ApplyContextDefaults();
			mStateMgr.PushState();
			if (!mStateMgr.CommitState())
			{
				mStateMgr.SetContext(context);
				return false;
			}
		}
		return true;
	}

	public void SetupDrawMode(int theDrawMode)
	{
		if (theDrawMode == 0)
		{
			mStateMgr.SetBlendStateState(mNormalState);
		}
		else
		{
			mStateMgr.SetBlendStateState(mAdditiveState);
		}
	}

	public Texture2D CreateTexture2D(int theWidth, int theHeight, PixelFormat theFormat, bool renderTarget, XNATextureData theTexData, XNATextureDataPiece[] theTexDataPiece)
	{
		GlobalMembers.gTotalGraphicsMemory += theWidth * theHeight * 4;
		SurfaceFormat xnaFormat = GetXnaFormat(theFormat);
		if (renderTarget)
		{
			return new RenderTarget2D(mDevice.GraphicsDevice, theWidth, theHeight);
		}
		return new Texture2D(mDevice.GraphicsDevice, theWidth, theHeight, false, xnaFormat);
	}

	public Texture2D CreateTexture2DFromData(byte[] data)
	{
		MemoryStream stream = new MemoryStream(data);
		Texture2D texture2D = Texture2D.FromStream(mDevice.GraphicsDevice, stream);
		GlobalMembers.gTotalGraphicsMemory += texture2D.Width * texture2D.Height * 4;
		return texture2D;
	}

	public DeviceImage GetOptimizedImage(Texture2D texture, bool commitBits, bool allowTriReps)
	{
		GlobalMembers.gTotalGraphicsMemory += texture.Width * texture.Height * 4;
		DeviceImage deviceImage = new DeviceImage();
		deviceImage.mApp = GlobalMembers.gSexyAppBase;
		deviceImage.mFileName = texture.Name;
		deviceImage.mWidth = texture.Width;
		deviceImage.mHeight = texture.Height;
		deviceImage.mHasAlpha = true;
		XNATextureData xNATextureData = new XNATextureData(this);
		deviceImage.SetRenderData(xNATextureData);
		xNATextureData.mWidth = texture.Width;
		xNATextureData.mHeight = texture.Height;
		xNATextureData.mTexPieceWidth = texture.Width;
		xNATextureData.mTexPieceHeight = texture.Height;
		xNATextureData.mTexVecWidth = 1;
		xNATextureData.mTexVecHeight = 1;
		xNATextureData.mPixelFormat = GetSexyFormat(texture.Format);
		xNATextureData.mMaxTotalU = 1f;
		xNATextureData.mMaxTotalV = 1f;
		xNATextureData.mImageFlags = deviceImage.GetImageFlags();
		xNATextureData.mOptimizedLoad = true;
		xNATextureData.mTextures[0].mWidth = texture.Width;
		xNATextureData.mTextures[0].mHeight = texture.Height;
		xNATextureData.mTextures[0].mTexture = texture;
		return deviceImage;
	}

	public SurfaceFormat GetXnaFormat(PixelFormat theFormat)
	{
		return theFormat switch
		{
			PixelFormat.PixelFormat_X8R8G8B8 => SurfaceFormat.Color, 
			PixelFormat.PixelFormat_A8R8G8B8 => SurfaceFormat.Color, 
			PixelFormat.PixelFormat_R5G6B5 => SurfaceFormat.Bgr565, 
			PixelFormat.PixelFormat_A4R4G4B4 => SurfaceFormat.Bgra4444, 
			_ => SurfaceFormat.Color, 
		};
	}

	public PixelFormat GetSexyFormat(SurfaceFormat theFormat)
	{
		return theFormat switch
		{
			SurfaceFormat.Color => PixelFormat.PixelFormat_A8R8G8B8, 
			SurfaceFormat.Bgr565 => PixelFormat.PixelFormat_R5G6B5, 
			SurfaceFormat.Bgra4444 => PixelFormat.PixelFormat_A4R4G4B4, 
			_ => PixelFormat.PixelFormat_A8R8G8B8, 
		};
	}

	public CompareFunction GetXNACompareFunc(Graphics3D.ECompareFunc func)
	{
		return func switch
		{
			Graphics3D.ECompareFunc.COMPARE_NEVER => CompareFunction.Never, 
			Graphics3D.ECompareFunc.COMPARE_LESS => CompareFunction.Less, 
			Graphics3D.ECompareFunc.COMPARE_EQUAL => CompareFunction.Equal, 
			Graphics3D.ECompareFunc.COMPARE_LESSEQUAL => CompareFunction.LessEqual, 
			Graphics3D.ECompareFunc.COMPARE_GREATER => CompareFunction.Greater, 
			Graphics3D.ECompareFunc.COMPARE_NOTEQUAL => CompareFunction.NotEqual, 
			Graphics3D.ECompareFunc.COMPARE_GREATEREQUAL => CompareFunction.GreaterEqual, 
			Graphics3D.ECompareFunc.COMPARE_ALWAYS => CompareFunction.Always, 
			_ => CompareFunction.Never, 
		};
	}

	public Blend GetXNABlendMode(Graphics3D.EBlendMode mode)
	{
		return mode switch
		{
			Graphics3D.EBlendMode.BLEND_DEFAULT => Blend.SourceAlpha, 
			Graphics3D.EBlendMode.BLEND_ZERO => Blend.Zero, 
			Graphics3D.EBlendMode.BLEND_ONE => Blend.One, 
			Graphics3D.EBlendMode.BLEND_SRCCOLOR => Blend.SourceColor, 
			Graphics3D.EBlendMode.BLEND_INVSRCCOLOR => Blend.InverseSourceColor, 
			Graphics3D.EBlendMode.BLEND_SRCALPHA => Blend.SourceAlpha, 
			Graphics3D.EBlendMode.BLEND_INVSRCALPHA => Blend.InverseSourceAlpha, 
			Graphics3D.EBlendMode.BLEND_DESTCOLOR => Blend.DestinationColor, 
			Graphics3D.EBlendMode.BLEND_INVDESTCOLOR => Blend.InverseDestinationColor, 
			Graphics3D.EBlendMode.BLEND_SRCALPHASAT => Blend.SourceAlphaSaturation, 
			_ => Blend.One, 
		};
	}

	public Matrix GetXNAMatrix(SexyMatrix4 mat)
	{
		return new Matrix(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);
	}

	public Matrix GetXNAMatrix(SexyMatrix3 mat)
	{
		return new Matrix(mat.m00, mat.m10, mat.m20, 0f, mat.m01, mat.m11, mat.m21, 0f, 0f, 0f, mat.m22, 0f, mat.m02, mat.m12, 0f, 1f);
	}

	public Microsoft.Xna.Framework.Color GetXNAColor(SexyFramework.Graphics.Color color)
	{
		return new Microsoft.Xna.Framework.Color(color.mRed, color.mGreen, color.mBlue, color.mAlpha);
	}

	public void CopyImageToTexture(ref Texture2D theTexture, int theTextureFormat, MemoryImage theImage, int offx, int offy, int texWidth, int texHeight, PixelFormat theFormat)
	{
		if (theTexture != null)
		{
			theTexture.SetData(theImage.GetBits());
		}
	}

	public void BufferedDrawPrimitive(int thePrimType, int thePrimCount, VertexPositionColorTexture[] theVertices, int theVertexSize, ulong theVertexFormat, Matrix transform)
	{
		CheckBatchAndCommit();
		int num = 0;
		switch (thePrimType)
		{
		case 4:
			while (thePrimCount > 0)
			{
				if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				ref VertexPositionColorTexture reference13 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference13 = theVertices[num++];
				ref VertexPositionColorTexture reference14 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference14 = theVertices[num++];
				ref VertexPositionColorTexture reference15 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
				reference15 = theVertices[num++];
				thePrimCount--;
			}
			break;
		case 5:
		{
			if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
			ref VertexPositionColorTexture reference7 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference7 = theVertices[num++];
			ref VertexPositionColorTexture reference8 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference8 = theVertices[num++];
			ref VertexPositionColorTexture reference9 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference9 = theVertices[num++];
			for (thePrimCount--; thePrimCount > 0; thePrimCount--)
			{
				ref VertexPositionColorTexture reference10 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex];
				reference10 = mBatchedTriangleBuffer[mBatchedTriangleIndex - 2];
				ref VertexPositionColorTexture reference11 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex + 1];
				reference11 = mBatchedTriangleBuffer[mBatchedTriangleIndex - 1];
				ref VertexPositionColorTexture reference12 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex + 2];
				reference12 = theVertices[num++];
				mBatchedTriangleIndex += 3;
			}
			break;
		}
		case 6:
		{
			if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
			int num2 = mBatchedTriangleIndex;
			ref VertexPositionColorTexture reference = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference = theVertices[num++];
			ref VertexPositionColorTexture reference2 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference2 = theVertices[num++];
			ref VertexPositionColorTexture reference3 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex++];
			reference3 = theVertices[num++];
			for (thePrimCount--; thePrimCount > 0; thePrimCount--)
			{
				ref VertexPositionColorTexture reference4 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex];
				reference4 = mBatchedTriangleBuffer[num2];
				ref VertexPositionColorTexture reference5 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex + 1];
				reference5 = mBatchedTriangleBuffer[mBatchedTriangleIndex - 1];
				ref VertexPositionColorTexture reference6 = ref mBatchedTriangleBuffer[mBatchedTriangleIndex + 2];
				reference6 = theVertices[num++];
				mBatchedTriangleIndex += 3;
			}
			break;
		}
		}
		if (mBatchedTriangleIndex + 3 > mBatchedTriangleSize)
		{
			DoCommitAllRenderState();
			FlushBufferedTriangles();
		}
	}

	public void DrawPrimitiveInternal<T>(int inPrimType, int inPrimCount, T[] inVertData, ulong inVertStride, ulong inVertFormat, bool inDoCommit, Matrix transform) where T : struct, IVertexType
	{
		int num = 0;
		switch (inPrimType)
		{
		case 4:
			num = inPrimCount * 3;
			break;
		case 5:
		case 6:
			num = inPrimCount + 2;
			break;
		case 3:
			num = inPrimCount + 1;
			break;
		}
		if (num == 0)
		{
			return;
		}
		if (inDoCommit)
		{
			CheckBatchAndCommit();
			DoCommitAllRenderState();
		}
		PrimitiveType primitiveType = PrimitiveType.TriangleList;
		switch (inPrimType)
		{
		case 4:
			primitiveType = PrimitiveType.TriangleList;
			break;
		case 5:
			primitiveType = PrimitiveType.TriangleStrip;
			break;
		case 6:
			return;
		case 3:
			primitiveType = PrimitiveType.LineStrip;
			break;
		}
		foreach (EffectPass pass in mBasicEffect.CurrentTechnique.Passes)
		{
			pass.Apply();
			try
			{
				mBasicEffect.GraphicsDevice.DrawUserPrimitives(primitiveType, inVertData, 0, inPrimCount);
			}
			catch (Exception)
			{
			}
		}
	}

	public void DrawIndexPrimitiveInternal<T>(int inPrimType, int inPrimCount, T[] inVertData, ulong inVertStride, ulong inVertFormat, bool inDoCommit, Matrix transform) where T : struct, IVertexType
	{
		if (inDoCommit)
		{
			CheckBatchAndCommit();
			DoCommitAllRenderState();
		}
		PrimitiveType primitiveType = PrimitiveType.TriangleList;
		switch (inPrimType)
		{
		case 4:
			primitiveType = PrimitiveType.TriangleList;
			break;
		case 5:
			primitiveType = PrimitiveType.TriangleStrip;
			break;
		case 6:
			return;
		case 3:
			primitiveType = PrimitiveType.LineStrip;
			break;
		}
		foreach (EffectPass pass in mBasicEffect.CurrentTechnique.Passes)
		{
			pass.Apply();
			try
			{
				mBasicEffect.GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, inVertData, 0, mBatchedTriangleIndex, mBatchedIndexBuffer, 0, inPrimCount);
			}
			catch (Exception)
			{
			}
		}
	}

	public VertexBuffer InternalCreateVertexBuffer(int inCount, VertexDeclaration vDec, BufferUsage usage)
	{
		return new VertexBuffer(mDevice.GraphicsDevice, vDec, inCount, usage);
	}

	public IndexBuffer InternalCreateIndexBuffer(int indexCount, IndexElementSize size, BufferUsage usage)
	{
		return new IndexBuffer(mDevice.GraphicsDevice, size, indexCount, usage);
	}

	public override Image SwapScreenImage(ref DeviceImage ioSrcImage, ref RenderSurface ioSrcSurface, uint flags)
	{
		throw new NotImplementedException();
	}

	public override void CopyScreenImage(DeviceImage ioDstImage, uint flags)
	{
		throw new NotImplementedException();
	}

	public override RenderEffect GetEffect(RenderEffectDefinition inDefinition)
	{
		return null;
	}

	public void SetOrientation(int Orientation)
	{
		mDevice.ApplyChanges();
	}
}
