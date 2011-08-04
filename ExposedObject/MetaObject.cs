using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExposedObject
{
    sealed class MetaObject : DynamicMetaObject
    {
        private bool isStatic;

        public MetaObject(Expression parameter, object o, bool staticBind) :
            base(parameter, BindingRestrictions.Empty, o)
        {
            isStatic = staticBind;
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var self = Expression;
            var exposed = (Exposed)Value;

            var argTypes = new Type[args.Length];
            var argExps = new Expression[args.Length];

            for (int i = 0; i < args.Length; ++i)
            {
                argTypes[i] = args[i].LimitType;
                argExps[i] = args[i].Expression;
            }

            var type = exposed.SubjectType;
            var method = type.GetMethod(binder.Name, GetBindingFlags(), null, argTypes, null);
            if (method == null)
            {
                throw new MissingMemberException(type.FullName, binder.Name);
            }

            var @this = isStatic
                            ? null
                            : Expression.Convert(Expression.Field(Expression.Convert(self, typeof(Exposed)), "o"), type);

            var target = Expression.Convert(Expression.Call(@this, method, argExps), binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var self = Expression;

            MemberExpression memberExpression = GetMemberExpression(self, binder.Name);

            var target = Expression.Convert(memberExpression, binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var self = Expression;

            MemberExpression memberExpression = GetMemberExpression(self, binder.Name);

            var target =
                Expression.Convert(
                    Expression.Assign(memberExpression, Expression.Convert(value.Expression, memberExpression.Type)),
                    binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        private MemberExpression GetMemberExpression(Expression self, string memberName)
        {
            MemberExpression memberExpression;
            var type = ((Exposed)Value).SubjectType;
            var @this = isStatic
                            ? null
                            : Expression.Convert(Expression.Field(Expression.Convert(self, typeof(Exposed)), "o"), type);
            var property = type.GetProperty(memberName, GetBindingFlags());
            if (property != null)
            {
                memberExpression = Expression.Property(@this, property);
            }
            else
            {
                var field = type.GetField(memberName, GetBindingFlags());
                if (field == null)
                {
                    throw new MissingMemberException(type.FullName, memberName);
                }

                memberExpression = Expression.Field(@this, field);
            }

            return memberExpression;
        }

        private BindingFlags GetBindingFlags()
        {
            return isStatic
                       ? BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                       : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }
    }
}
