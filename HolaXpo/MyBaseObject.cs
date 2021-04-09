using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;

namespace HolaXpo
{
    [NonPersistent, MemberDesignTimeVisibility(false)]
    public class MyBaseObject : XPObject
    {
        public MyBaseObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
    }
}
