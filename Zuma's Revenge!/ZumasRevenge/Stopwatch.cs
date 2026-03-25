using System;

namespace ZumasRevenge;

public class Stopwatch
{
	private string text;

	private int start;

	public Stopwatch(string msg)
	{
		text = msg;
		start = DateTime.Now.Millisecond;
	}

	~Stopwatch()
	{
	}
}
