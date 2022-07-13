using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MS.Debuggers.DbgEng;
using System.Collections;
using System.Threading;

namespace ASDumpAnalyzer
{
    public class ASDump
    {
        public static string GetQuery(DbgFrame[] stack, UpdateProgressDelegate updateProgress)
        {
            Dictionary<string, int> possibleQueries = new Dictionary<string, int>();

            int stackLength = stack.Length;

            for (int i = 0; i < stack.Length; i++)
            {
                DbgFrame frame = stack[i];
                string functionName = frame.FunctionName;

                foreach (DbgArgument arg in frame.Arguments)
                {
                    string parameterName = arg.Name;

                    if ("PCExecutionContext*" == arg.Type.Name)
                    {
                        //if (8 == arg.Type.Size)
                        //{
                            try
                            {
                                string q;
                                if (GetQueryFromContext(arg.Dereference(), out q))
                                {
                                    int n;
                                    if (possibleQueries.TryGetValue(q, out n))
                                        possibleQueries[q] = n + 1;
                                    else
                                        possibleQueries.Add(q, 1);
                                    break;
                                }
                            }
                            catch (Exception) { } //Eat the exception and hope that the query will be found on another instance of PCExecutionContext
                        //}
                        //else if (4 == arg.Type.Size)
                        //{
                        //    throw new NotImplementedException();
                        //}
                    }
                }
                //Thread.Sleep(100);
                if (null != updateProgress)
                {
                    updateProgress(((double)i) / stackLength);
                }
            }
            if (0 == possibleQueries.Count)
                return "Failed to load a query from call stack";
            else
            {
                KeyValuePair<string, int>[] queries = new KeyValuePair<string, int>[possibleQueries.Count];
                int q = 0;
                foreach (KeyValuePair<string, int> pair in possibleQueries)
                {
                    queries[q] = pair;
                    q++;
                }

                for (int i = 0; i < queries.Length; i++)
                    for (int j = i + 1; j < queries.Length; j++)
                        if (queries[i].Value < queries[j].Value)
                        {
                            KeyValuePair<string, int> temp = queries[i];
                            queries[i] = queries[j];
                            queries[j] = temp;
                        }

                StringBuilder result = new StringBuilder(4 * queries.Length);
				if (queries.Length > 1)
				{
					for (int i = 0; i < queries.Length; i++)
					{
						result.Append("====== Result with probability p=");
						result.Append(queries[i].Value);
						result.Append(" ======\n");
						result.AppendLine(queries[i].Key);
					}
				}
				else 
				{
					result.Append(queries[0].Key);
				}
                return result.ToString();
            }
        }


        private static bool GetQueryFromContext(DbgObject pcec, out string query)
        {
            try
            {
                DbgObject pSession = pcec.ReadField("m_spSession");
                if (pSession.IsNull)
                {
                    query = "Cannot find PXSession object";
                    return false;
                }
                DbgObject session = pSession.Dereference("PXSession");
                DbgObject pLastRequest = session.ReadField("m_strLastRequest");

                if (pLastRequest.IsNull)
                {
                    query = "Cannot find the m_strLastRequest object";
                    return false;
                }
                DbgObject lastRequest = pLastRequest.Dereference("char*");

                if (lastRequest.IsNull)
                {
                    query = "m_strLastRequest points to a null";
                    return false;
                }

                query = lastRequest.ReadAsStringUnicode();

                return true;
            }
            catch (COMException)
            {
                query = "COMException during query extraction";
                return false;
            }
            catch (InvalidCastException)
            {
                query = "InvalidCastException during query extraction";
                return false;
            }
        }
    }
}