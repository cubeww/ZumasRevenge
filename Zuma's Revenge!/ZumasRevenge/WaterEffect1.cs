using SexyFramework.Graphics;

namespace ZumasRevenge;

public class WaterEffect1 : Effect
{
	public WaterEffect1()
	{
		mResGroup = "";
		Reset("");
	}

	protected void SetupShoreWaves(int x, int y, bool mirror, float vx, float vy)
	{
	}

	public override string GetName()
	{
		return "WaterEffect1";
	}

	public override void Update()
	{
	}

	public override void Reset(string level_id)
	{
		mUpdateCount++;
	}

	public override void DrawPriority(Graphics g, int priority)
	{
	}
}
