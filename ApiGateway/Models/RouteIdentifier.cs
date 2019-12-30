using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiGateway.Models
{
    public class RouteIdentifier
    {
        private readonly Lazy<string[]> _tokenizedPath;
        
        private RouteIdentifier()
        {
            // lazy initialize this so we only string split if we look at the property
            _tokenizedPath = new Lazy<string[]>(() => string.IsNullOrWhiteSpace(Path)
                ? Array.Empty<string>()
                : Path.TrimStart('/').Split('/'));
        }

        public RouteIdentifier(HttpRequest request) : this()
        {
            HttpMethod = request.Method;
            Path = request.Path.Value;
        }

        public RouteIdentifier(string httpMethod, string path) : this()
        {
            HttpMethod = httpMethod;
            Path = path;
        }

        public string HttpMethod { get; }

        public string Path { get; }

        private string[] TokenizedPath => _tokenizedPath.Value;


        public bool TokenizedRouteEquals(RouteIdentifier other)
        {
            return TokenizedRouteEqualityComparer.Equals(this, other);
        }

        public override string ToString()
        {
            return $"{HttpMethod} {Path}";
        }

        public static IEqualityComparer<RouteIdentifier> TokenizedRouteEqualityComparer => new TokenizedRouteIdentifierComparer();

        public static IComparer<RouteIdentifier> NumberOfParameterizedTokens => new NumberOfParameterizedTokensComparer();

        private class TokenizedRouteIdentifierComparer : IEqualityComparer<RouteIdentifier>
        {
            private static bool IsTokenUnique(string a, string b)
            {
                // Paths can look like this `tickets/{id}` or `tickets/unprocessed`
                // these two will be identical from a routing point of view 
                // because `unprocessed` could fulfill the url parameter placeholder `{id}`
                // so, to be unique they must both be not equal and not placeholders

                return !string.Equals(a, b, StringComparison.OrdinalIgnoreCase)
                       && !(a.StartsWith("{") || a.EndsWith("}"))
                       && !(b.StartsWith("{") || b.EndsWith("}"));
            }

            public bool Equals(RouteIdentifier a, RouteIdentifier b)
            {
                if (a == null || b == null
                    // method must be non-null and equal
                    || string.IsNullOrWhiteSpace(a.HttpMethod) || string.IsNullOrWhiteSpace(b.HttpMethod)
                    || !string.Equals(a.HttpMethod, b.HttpMethod, StringComparison.OrdinalIgnoreCase)

                    // path must be non-null and token-wise equal
                    || a.TokenizedPath == null || b.TokenizedPath == null
                    || a.TokenizedPath.Length != b.TokenizedPath.Length)
                {
                    return false;
                }

                for (var i = 0; i < a.TokenizedPath.Length; i++)
                {
                    if (IsTokenUnique(a.TokenizedPath[i], b.TokenizedPath[i])) return false;
                }

                return true;
            }


            public int GetHashCode(RouteIdentifier obj)
            {
                // this will be used by the Dictionary<> to determine if the objects are "equal-enough to be worth calling .Equals"
                // so this should be a trimmed-down version of .Equals basically doing any quick pre-condition checks.

                var hash = 0;
                // will just take the HashCode for the HTTP Method string
                if (obj.HttpMethod != null) hash += obj.HttpMethod.ToUpper().GetHashCode();

                // then add the number of tokens in the path because if these are not equal, then the path cannot be equal.
                if (obj.TokenizedPath != null) hash += obj.TokenizedPath.Length;
                return hash;
            }
        }


        /// <summary>
        /// Comparer for RouteIdentifier which can be used to order by the number of parameters in the route.
        /// For example `GET /tickets/{id}` has 2 tokens `tickets` and `{id}` one of which is parameterized `{id}`.
        /// So `GET /tickets/{id}` is more parameterized (1) than `GET /tickets/unprocessed` (0).
        /// </summary>
        private class NumberOfParameterizedTokensComparer : IComparer<RouteIdentifier>
        {
            private bool IsTokenParameterized(string token)
            {
                return token.StartsWith("{") && token.EndsWith("}");
            }

            private int GetNumberOfParameterizedTokens(RouteIdentifier route)
            {
                var numberOfParameterizedTokens = 0;

                if (route == null) return numberOfParameterizedTokens;

                foreach (var token in route.TokenizedPath)
                {
                    if (IsTokenParameterized(token))
                        numberOfParameterizedTokens++;
                }

                return numberOfParameterizedTokens;
            }

            public int Compare(RouteIdentifier x, RouteIdentifier y)
            {
                return GetNumberOfParameterizedTokens(x) - GetNumberOfParameterizedTokens(y);
            }
        }

    }
}
