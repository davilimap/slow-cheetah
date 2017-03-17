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

        public static JObject GetTransformWithPrimitiveReplace
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

        public static JObject GetTransformWithPrimitiveAdds
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

        public static JObject GetExpectedWithPrimitiveAdds
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

        public static JObject GetTransformWithObjectAdds
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

        public static JObject GetExpectedWithObjectAdds
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

        public static JObject GetDocumentWithObjects
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": {
                        },
                        ""B"": {
                            ""B1"" : 1,
                            ""B2"" : 2
                        },
                        ""C"": {
                            ""C1"": {
                                ""C11"":true
                            },
                            ""C2"": 2
                        }
                    }");
            }
        }

        public static JObject GetTransformWithObjectMerge
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": {
                            ""A1"": ""New""
                        },
                        ""B"": {
                            ""B1"" : 10,
                            ""B3"" : 30
                        },
                        ""C"": {
                            ""C1"": {
                                ""C12"":false
                            },
                            ""C3"": {
                                ""Added"": true
                            }
                        }
                    }");
            }
        }

        public static JObject GetExpectedWithObjectMerge
        {
            get
            {
                return JObject.Parse(
                    @"{
                        ""A"": {
                            ""A1"": ""New""
                        },
                        ""B"": {
                            ""B1"" : 10,
                            ""B2"" : 2,
                            ""B3"" : 30
                        },
                        ""C"": {
                            ""C1"": {
                                ""C11"":true,
                                ""C12"":false
                            },
                            ""C2"": 2,
                            ""C3"": {
                                ""Added"": true
                            }
                        }
                    }");
            }
        }
    }
}
