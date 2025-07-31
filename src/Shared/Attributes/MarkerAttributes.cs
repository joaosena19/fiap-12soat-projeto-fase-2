using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateRootAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class ValueObjectAttribute : Attribute { }
}
