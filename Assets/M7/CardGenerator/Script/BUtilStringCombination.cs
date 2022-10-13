using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace M7.GameRuntime
{
    public class BUtilStringCombination
	{
		//public void CombinationEquipment(params string[][] arrays)
	    //{
		//    CombinationEquipment(arrays, 0, new string[arrays.Length]);
	    //}
	
		List<string> resultStr = new List<string>();
		public List<string> CombinationEquipment(string[][] arrays, int depth, string[] current)
		{
		    for (int i=0; i < arrays[depth].Length; i++)
		    {
			    current[depth] = arrays[depth][i];

			    if (depth < arrays.Length-1)
			    {
				    CombinationEquipment(arrays, depth+1, current);
			    }
			    else
			    {
			    	StringBuilder builder = new StringBuilder();
				    foreach (string x in current)
				    {
					    builder.Append(x);
					    builder.Append(", ");
				    }
				    builder.Length--;
				    //Debug.Log (builder.ToString());
				    resultStr.Add(builder.ToString());
			    }
		    }
			return resultStr;
	    }
    }
}
