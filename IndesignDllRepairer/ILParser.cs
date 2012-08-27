using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IndesignDllRepairer
{
    class ILParser
    {
        protected delegate bool state_func_t(string input);
	    protected state_func_t mState;

	    protected string mClass;
	    protected string mReturnType;

	    protected Regex mClassRx = new Regex(@"^\s*\.class.* ([\w\.]+)\s*$");
	    protected Regex mMethodRx = new Regex(@"^\s*\.method");
	    protected Regex mMethodNameRx = new Regex(@"^\s*Duplicate");
	    protected Regex mReturnTypeRx = new Regex(@"^\s*instance class ([\w\.]+)");
	    protected Regex mEndBraceRx = new Regex(@"\}");

	    protected Queue<String> mOutBuf;
	    protected StreamWriter mOutFile;

	    public ILParser()
	    {
	    }

	    public void Parse(string path)
	    {
		    Console.WriteLine("Starting parsing");
		    mState = NullState;
		    StreamReader file = new StreamReader(path);
		    mOutFile = new StreamWriter(path + ".out");
            mOutBuf = new Queue<string>();

            string line;
		    while ((line = file.ReadLine()) != null)
		    {
			    mOutBuf.Enqueue(line);

			    Console.WriteLine(line);
			    Console.WriteLine(mOutBuf);
			    Console.WriteLine(new string('=', 80));

			    //if (!mState(to!string(buf)))
			    //{
			    //	break;
			    //}
		    }
	    }

	    void DumpBuffer()
	    {
		    while (mOutBuf.Count > 0)
		    {
			    string line = mOutBuf.Dequeue();
			    mOutFile.WriteLine(line);
		    }
	    }

	    bool NullState(string input)
	    {
		    MatchCollection m = mClassRx.Matches(input);
		    if (m.Count > 0)
		    {
			    mClass = m[0].Value;
			    mState = ClassState;
		    }
		
		    return true;
	    }

	    bool ClassState(string input)
	    {
		    MatchCollection m = mMethodRx.Matches(input);
		    if (m.Count > 0)
		    {
			    mState = MethodState;
			    mReturnType = "";
		    }

		    m = mEndBraceRx.Matches(input);
		    if (m.Count > 0)
		    {
			    //DumpBuffer();
			    mState = NullState;
		    }

		    return true;
	    }

	    bool MethodState(string input)
	    {
		    MatchCollection m = mReturnTypeRx.Matches(input);
		    //if (mClass == "InDesign.MixedInk")
		    //{
		    //	writefln("%s: method match: %s", input, m);
		    //}
		
		    if (m.Count > 0)
		    {
			    mReturnType = m[0].Value;
			    return true;
		    }

		    m = mMethodNameRx.Matches(input);
		    if (m.Count > 0)
		    {
			    if (mClass != mReturnType)
			    {
				    //writeln("=".replicate(80));
				    //writeln(mOutBuf);
				    //ReplaceReturnType();
				    //writefln("%s::Duplicate returns %s", mClass, mReturnType);
			    }
			    return true;
		    }
		
		    m = mEndBraceRx.Matches(input);
		    if (m.Count > 0)
		    {
                DumpBuffer();
			    mState = ClassState;
		    }

		    return true;
	    }

	    void ReplaceReturnType()
	    {
		    //writeln("=".replicate(80));
		    //writeln("Replacing return type, buffer length=", mOutBuf.length);
		    Regex rx = new Regex(@"[\w\.]+\s*$");

		    while (mOutBuf.Count > 0)
		    {
			    string line = mOutBuf.Dequeue();
			    MatchCollection m = mReturnTypeRx.Matches(line);
			    if (m.Count > 0)
			    {
                    line = rx.Replace(line, mClass);
                }

			    mOutFile.WriteLine(line);
		    }
	    }
    }
}
