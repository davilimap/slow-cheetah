// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.Tests.JDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public static class JdtTestUtilities
    {
        public static JObject GetDocumentObjectWithPrimitives
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": 1,
                        ""B"": true,
                        ""C"": {
                            ""C1"": 10
                        }
                    }");
            }
        }

        public static JObject GetTransformObjectWithPrimitiveSubs
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": 20,
                        ""B"": null,
                        ""C"": ""Replaced""
                    }");
            }
        }

        public static JObject GetTransformObjectWithPrimitiveAdds
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""D"": true,
                        ""E"": 10,
                        ""F"": null
                    }");
            }
        }

        public static JObject GetExpectedResultWithPrimitiveAdds
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": 1,
                        ""B"": true,
                        ""C"": {
                            ""C1"": 10
                        },
                        ""D"": true,
                        ""E"": 10,
                        ""F"": null
                    }");
            }
        }

        public static JObject GetTransformObjectWithObjectAdds
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""D"": {
                        },
                        ""E"": {
                            ""E1"": 1,
                            ""E2"": true,
                            ""E3"": ""E3""
                        },
                        ""F"": {
                            ""F1"": {
                            },
                            ""F2"": null,
                            ""F3"": {
                                ""F31"": 10,
                                ""F32"": null
                            }
                        }
                    }");
            }
        }

        public static JObject GetExpectedResultWithObjectAdds
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": 1,
                        ""B"": true,
                        ""C"": {
                            ""C1"": 10
                        },
                        ""D"": {
                        },
                        ""E"": {
                            ""E1"": 1,
                            ""E2"": true,
                            ""E3"": ""E3""
                        },
                        ""F"": {
                            ""F1"": {
                            },
                            ""F2"": null,
                            ""F3"": {
                                ""F31"": 10,
                                ""F32"": null
                            }
                        }
                    }");
            }
        }
    }
}
