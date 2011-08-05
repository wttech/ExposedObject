using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExposedObject
{
    /// <summary>
    /// Represents the dynamic binding and a binding logic of an object participating in the dynamic binding.
    /// </summary>
    internal sealed class MetaObject : DynamicMetaObject
    {
        /// <summary>
        /// Should this <see cref="MetaObject"/> bind to <see langword="static"/> or instance methods and fields.
        /// </summary>
        private readonly bool isStatic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaObject"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression representing this <see cref="DynamicMetaObject"/> during the dynamic binding process.
        /// </param>
        /// <param name="value">
        /// The runtime value represented by the <see cref="DynamicMetaObject"/>.
        /// </param>
        /// <param name="staticBind">
        /// Should this MetaObject bind to <see langword="static"/> or instance methods and fields.
        /// </param>
        public MetaObject(Expression expression, object value, bool staticBind) :
            base(expression, BindingRestrictions.Empty, value)
        {
            isStatic = staticBind;
        }

        /// <summary>
        /// Performs the binding of the dynamic invoke member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="InvokeMemberBinder"/> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="args">
        /// An array of <see cref="DynamicMetaObject"/> instances - arguments to the invoke member operation.
        /// </param>
        /// <returns>
        /// The new <see cref="DynamicMetaObject"/> representing the result of the binding.
        /// </returns>
        /// <exception cref="MissingMemberException">
        /// There is an attempt to dynamically access a class member that does not exist.
        /// </exception>
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
                            : Expression.Convert(Expression.Field(Expression.Convert(self, typeof(Exposed)), "value"), type);

            var target = Expression.Convert(Expression.Call(@this, method, argExps), binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="GetMemberBinder"/> that represents the details of the dynamic operation.
        /// </param>
        /// <returns>
        /// The new <see cref="DynamicMetaObject"/> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var self = Expression;

            var memberExpression = GetMemberExpression(self, binder.Name);

            var target = Expression.Convert(memberExpression, binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        /// <summary>
        /// Performs the binding of the dynamic set member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="SetMemberBinder"/> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="value">
        /// The <see cref="DynamicMetaObject"/> representing the value for the set member operation.
        /// </param>
        /// <returns>
        /// The new <see cref="DynamicMetaObject"/> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var self = Expression;

            var memberExpression = GetMemberExpression(self, binder.Name);

            var target =
                Expression.Convert(
                    Expression.Assign(memberExpression, Expression.Convert(value.Expression, memberExpression.Type)),
                    binder.ReturnType);
            var restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Exposed));

            return new DynamicMetaObject(target, restrictions);
        }

        /// <summary>
        /// Generates the <see cref="Expression"/> for accessing a member by name.
        /// </summary>
        /// <param name="self">
        /// The <see cref="Expression"/> for accessing the <see cref="Exposed"/> instance.
        /// </param>
        /// <param name="memberName">
        /// The member name.
        /// </param>
        /// <returns>
        /// <see cref="MemberExpression"/> for accessing a member by name.
        /// </returns>
        /// <exception cref="MissingMemberException">
        /// </exception>
        private MemberExpression GetMemberExpression(Expression self, string memberName)
        {
            MemberExpression memberExpression;
            var type = ((Exposed)Value).SubjectType;
            var @this = isStatic
                            ? null
                            : Expression.Convert(Expression.Field(Expression.Convert(self, typeof(Exposed)), "value"), type);
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

        /// <summary>
        /// Returns <see cref="BindingFlags"/> for member search.
        /// </summary>
        /// <returns>
        /// Static or instance flags depending on <see cref="isStatic"/>.
        /// </returns>
        private BindingFlags GetBindingFlags()
        {
            return isStatic
                       ? BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                       : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }
    }
}
