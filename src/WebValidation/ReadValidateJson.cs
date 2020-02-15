using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebValidation
{
    public partial class Test : IDisposable
    {
        /// <summary>
        /// Load the requests from json files
        /// </summary>
        /// <param name="fileList">list of files to load</param>
        /// <returns>sorted List or Requests</returns>
        private List<Request> LoadValidateRequests(List<string> fileList)
        {
            List<Request> list;
            List<Request> fullList = new List<Request>();

            // read each json file
            foreach (string inputFile in fileList)
            {
                list = ReadJson(inputFile);

                // add contents to full list
                if (list != null && list.Count > 0)
                {
                    fullList.AddRange(list);
                }
            }

            // return null if can't read and validate the json files
            if (fullList == null || fullList.Count == 0 || !ValidateJson(fullList))
            {
                return null;
            }

            // return sorted list
            return fullList.OrderBy(x => x.SortOrder).ThenBy(x => x.Index).ToList();
        }

        /// <summary>
        /// Load performance targets from json
        /// </summary>
        /// <returns>Dictionary of PerfTarget</returns>
        private Dictionary<string, PerfTarget> LoadPerfTargets()
        {
            const string perfFileName = "TestFiles/perfTargets.json";

            if (File.Exists(perfFileName))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, PerfTarget>>(File.ReadAllText(perfFileName));
            }

            // return empty dictionary - perf targets are not required
            return new Dictionary<string, PerfTarget>();
        }

        /// <summary>
        /// Read a json test file
        /// </summary>
        /// <param name="file">file path</param>
        /// <returns>List of Request</returns>
        public List<Request> ReadJson(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            // check for file exists
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Console.WriteLine($"File Not Found: {file}");
                return null;
            }

            // read the file
            string json = File.ReadAllText(file);

            // check for empty file
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"Unable to read file {file}");
                return null;
            }

            try
            {
                // deserialize json into a list (array)
                List<Request> list = JsonConvert.DeserializeObject<List<Request>>(json);

                if (list != null && list.Count > 0)
                {
                    List<Request> l2 = new List<Request>();

                    foreach (Request r in list)
                    {
                        // Add the default perf targets if exists
                        if (r.PerfTarget != null && r.PerfTarget.Targets == null)
                        {
                            if (Targets.ContainsKey(r.PerfTarget.Category))
                            {
                                r.PerfTarget.Targets = Targets[r.PerfTarget.Category].Targets;
                            }
                        }

                        l2.Add(r);
                    }
                    // success
                    return l2.OrderBy(x => x.SortOrder).ThenBy(x => x.Index).ToList();
                }

                Console.WriteLine("Invalid JSON file");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // couldn't read the list
            return null;
        }

        /// <summary>
        /// Validate all of the rules
        /// </summary>
        /// <param name="requests">list of Request</param>
        /// <returns></returns>
        private static bool ValidateJson(List<Request> requests)
        {
            int ndx = 0;

            // validate each rule
            foreach (Request r in requests)
            {
                if (!ValidateRequest(r, out string message))
                {
                    Console.WriteLine($"Error: Invalid json\n\t{JsonConvert.SerializeObject(r, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })}\n\t{message}");
                    return false;
                }

                // set the index property
                r.Index = ndx;
                ndx++;

                // set default sort order
                if (r.SortOrder == null)
                {
                    r.SortOrder = 100;
                }
            }

            // validated
            return true;
        }

        /// <summary>
        /// Validate the request rules
        /// </summary>
        /// <param name="r">Request</param>
        /// <param name="message">out string error message</param>
        /// <returns></returns>
        private static bool ValidateRequest(Request r, out string message)
        {
            // null check
            if (r == null)
            {
                message = "cannot validate null request";
                return false;
            }

            // validate the request path
            if (!ValidateRequestPath(r.Url, out message))
            {
                return false;
            }

            // validate the verb
            if (!ValidateRequestVerb(r.Verb, out message))
            {
                return false;
            }

            // nothing to check
            if (r.Validation == null)
            {
                message = string.Empty;
                return true;
            }

            // validate http status code
            if (r.Validation.Code < 200 || r.Validation.Code > 599)
            {
                message = "statusCode: invalid status code: " + r.Validation.ToString();
                return false;
            }

            // validate json array parameters
            if (r.Validation.JsonArray != null && !ValidateRequestJsonArray(r.Validation.JsonArray, out message))
            {
                return false;
            }

            // TODO - validate other params

            // validated
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// validate the request HTTP verb
        /// </summary>
        /// <param name="verb">string</param>
        /// <param name="message">out string error message</param>
        /// <returns></returns>
        private static bool ValidateRequestVerb(string verb, out string message)
        {
            // verb must be in this list
            if (!(new List<string> { "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "OPTIONS", "CONNECT", "PATCH"}).Contains(verb))
            {
                message = "verb: invalid verb: " + verb;
                return false;
            }

            // validated
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// validate request path
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="message">out string error message</param>
        /// <returns></returns>
        private static bool ValidateRequestPath(string path, out string message)
        {
            // url is required
            if (string.IsNullOrWhiteSpace(path))
            {
                message = "url: url is required";
                return false;
            }

            // url must begin with /
            if (!path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                message = "url: url must begin with /";
                return false;
            }

            // validated
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Validate the json array rules
        /// </summary>
        /// <param name="rule">JsonArray</param>
        /// <param name="message">error message</param>
        /// <returns>bool success (out message)</returns>
        private static bool ValidateRequestJsonArray(JsonArray rule, out string message)
        {
            // null check
            if (rule == null)
            {
                message = "jsonArray: cannot validate null request";
                return false;
            }

            if (rule.CountIsZero)
            {
                // duplicate rule check
                if (rule.Count != 0 ||
                    rule.MinCount != 0 ||
                    rule.MaxCount != 0)
                {
                    message = "jsonArray: cannot specify multiple array checks";
                    return false;
                }
            }
            else
            {
                // must be >= 0 (0 is default
                if (rule.Count < 0 || rule.MinCount < 0 || rule.MaxCount < 0)
                {
                    message = "jsonArray: count parameters must be >= 0";
                    return false;
                }

                // no rules
                if (rule.Count == 0 && rule.MinCount == 0 && rule.MaxCount == 0)
                {
                    message = "jsonArray: must specify an array check";
                    return false;
                }

                if (rule.Count > 0)
                {
                    // can't combine count with min / max
                    if (rule.MinCount != 0 || rule.MaxCount != 0)
                    {
                        message = "jsonArray: cannot combine count with minCount / maxCount";
                        return false;
                    }
                }
                else
                {
                    // maxCount must be > minCount - if equal, use Count
                    if (rule.MaxCount <= rule.MinCount)
                    {
                        message = "jsonArray: MaxCount must be > MinCount";
                        return false;
                    }
                }
            }

            // validated
            message = string.Empty;
            return true;
        }
    }
}
