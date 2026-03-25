using System;
using System.Collections.Generic;

namespace ZumasRevenge;

public class MosquitoBall : IDisposable
{
	public List<Mosquito> mMosquitoes = new List<Mosquito>();

	public virtual void Dispose()
	{
		mMosquitoes.Clear();
	}
}
