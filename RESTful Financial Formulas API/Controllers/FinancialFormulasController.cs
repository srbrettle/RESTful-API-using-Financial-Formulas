using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using srbrettle.FinancialFormulas;
using System.Reflection;

namespace RESTful_Financial_Formulas_API.Controllers
{
    /// <summary>
    /// Class to act as an API for FinancialFormulas
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FinancialFormulasController : Controller
    {        
        /// <summary>
        /// Triggered by GET .../api/FinancialFormulas
        /// </summary>
        /// <returns>Default instructional message</returns>
        [HttpGet]
        public string Get()
        {
            return "Please enter query (FinancialFormulas Function Name) and parameters " +
                "e.g localhost:63995/api/FinancialFormulas/CalcAssets/10/20";
        }

        /// <summary>
        /// Triggered by GET .../api/FinancialFormulas/{FunctionName}/{FunctionParam1}/{FunctionParam2}...
        /// </summary>
        /// <param name="query">Function Name from the FinancialFormulas package</param>
        /// <param name="p1">First parameter for function call</param>
        /// <param name="p2">Second parameter for function call if required</param>
        /// <param name="p3">Third parameter for function call if required</param>
        /// <param name="p4">Fourth parameter for function call if required</param>
        /// <returns>Value from FinancialFormulas function or Error message</returns>
        [HttpGet]
        [Route("{query}/{p1}/{p2?}/{p3?}/{p4?}")]
        public string GetResult(string query, double p1, double? p2 = null, double? p3 = null, double? p4 = null)
        {
            var listOfMethods = GetFinancialFormulasMethods();

            bool? isQueryInputValid = CheckQueryInput(query, listOfMethods);
            if (isQueryInputValid == null)
            {
                // Query not input
                return "Error, query not entered";
            }
            else if (!(bool)isQueryInputValid)
            {
                // Invalid query input
                if (!AttemptQueryCorrection(query, listOfMethods, out query))
                {
                    return "Error, check query name and parameter values";
                }
            }

            // Construct parameters object
            object[] parameters;
            if (p2 == null)
            {
                parameters = new object[] { p1 };
            }
            else if (p3 == null)
            {
                parameters = new object[] { p1, p2 };
            }
            else if (p4 == null)
            {
                parameters = new object[] { p1, p2, p3 };
            }
            else
            {
                parameters = new object[] { p1, p2, p3, p4 };
            }

            // Call the FinancialFormulas function
            return CallFormulaFunction(query, parameters);
        }

        /// <summary>
        /// Triggered by GET .../api/FinancialFormulas/conventional?query={FunctionName}&p1={FunctionParam1}&p2={FunctionParam2}...
        /// </summary>
        /// <param name="query">Function Name from the FinancialFormulas package</param>
        /// <param name="p1">First parameter for function call</param>
        /// <param name="p2">Second parameter for function call if required</param>
        /// <param name="p3">Third parameter for function call if required</param>
        /// <param name="p4">Fourth parameter for function call if required</param>
        /// <returns>Value from FinancialFormulas function or Error message</returns>
        [HttpGet]
        [Route("conventional")]
        public string GetResultConventional(string query, string p1, string p2 = null, string p3 = null, string p4 = null)
        {
            var listOfMethods = GetFinancialFormulasMethods();

            bool? isQueryInputValid = CheckQueryInput(query, listOfMethods);
            if (isQueryInputValid == null)
            {
                // Query not input
                return "Error, query not entered";
            }
            else if (!(bool)isQueryInputValid)
            {
                // Invalid query input
                if (!AttemptQueryCorrection(query, listOfMethods, out query)) {
                    return "Error, check query name and parameter values";
                }
            }

            // Construct parameters object
            object[] parameters;
            if (p2 == null)
            {
                parameters = new object[] { double.Parse(p1) };
            }
            else if (p3 == null)
            {
                parameters = new object[] { double.Parse(p1), double.Parse(p2) };
            }
            else if (p4 == null)
            {
                parameters = new object[] { double.Parse(p1), double.Parse(p2), double.Parse(p3) };
            }
            else
            {
                parameters = new object[] { double.Parse(p1), double.Parse(p2), double.Parse(p3), double.Parse(p4) };
            }

            // Call the FinancialFormulas function
            return CallFormulaFunction(query, parameters);
        }

        /// <summary>
        /// Check that the query input is valid
        /// </summary>
        /// <param name="query">Query input from GET</param>
        /// <returns>Null if query is null or empty, false if invalid but potentially correctable, true if passed validations</returns>
        private bool? CheckQueryInput(string query, List<string> listOfMethods)
        {
            // Check query is not not null or empty
            if (String.IsNullOrEmpty(query))
            {
                // Irrecoverable, return null
                return null;
            }

            // Check first letter is uppercase
            if (char.IsLower(query.First()))
            {
                return false;
            }

            // Check that query starts with "calc"/"Calc"
            if (!query.ToLower().StartsWith("calc"))
            {
                return false;
            }

            if (!listOfMethods.Contains(query))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attemp to correct an invalid query
        /// </summary>
        /// <param name="inputQuery">Query input by the user</param>
        /// <param name="listOfMethods">List of function names from FinancialFormulas</param>
        /// <param name="query">Output query that corrections are applied to</param>
        /// <returns>Query with any corrections applied</returns>
        private bool AttemptQueryCorrection(string inputQuery, List<string> listOfMethods, out string query)
        {
            // Initialise query
            query = inputQuery;

            if (char.IsLower(query.First()))
            {
                // Make first letter Upper
                query = inputQuery.First().ToString().ToUpper() + inputQuery.Substring(1);
            }

            if (!query.StartsWith("Calc"))
            {
                // Ensure query starts with "Calc"
                query = $"Calc{query}";
            }

            // Check if attempt to correct has resulted in a valid function from FinancialFormulas
            if (!listOfMethods.Contains(query))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Calls the FinancialFormulas function using reflection
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>Result from called function or an error message</returns>
        private string CallFormulaFunction(string query, object[] parameters)
        {
            try
            {               
                // Use reflection to call function from FinancialFormulas
                Assembly info = typeof(FinancialFormulas).Assembly;
                Type t = info.GetType("srbrettle.FinancialFormulas.FinancialFormulas");
                MethodInfo m = t.GetMethod(query);
                object result = m.Invoke(null, parameters);

                return result.ToString();
            }
            catch { }
            return "Error, check query name and parameter values";
        }

        /// <summary>
        /// Populates a list of the function names from FinancialFormulas
        /// </summary>
        /// <returns>List of the FinancialFormulas function names</returns>
        private List<string> GetFinancialFormulasMethods()
        {
            var listOfMethods = new List<string>();

            Assembly info = typeof(FinancialFormulas).Assembly;
            Type t = info.GetType("srbrettle.FinancialFormulas.FinancialFormulas");
            MethodInfo[] methods = t.GetMethods();

            foreach (var method in methods)
            {
                listOfMethods.Add(method.Name);
            }

            return listOfMethods;
        }
    }
}
