using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public static class Activator
	{
		private delegate T ObjectActivator<out T>(params object[] args);

		private static readonly Dictionary<Type, object> Cached = new Dictionary<Type, object>();

		public static T Activate<T>(IPublishedContent model)
		{
			if (!Cached.ContainsKey(typeof(T)))
			{
				var ctor = typeof(T).GetConstructors().First();
				var createdActivator = GetActivator<T>(ctor);

				Cached.Add(typeof(T), createdActivator);
			}

			return ((ObjectActivator<T>)Cached[typeof(T)])(model);
		}

		private static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
		{
			var paramsInfo = ctor.GetParameters();

			//create a single param of type object[]
			var param = Expression.Parameter(typeof(object[]), "args");

			var argsExp = new Expression[paramsInfo.Length];

			//pick each arg from the params array 
			//and create a typed expression of them
			for (var i = 0; i < paramsInfo.Length; i++)
			{
				var index = Expression.Constant(i);
				var paramType = paramsInfo[i].ParameterType;

				var paramAccessorExp =
					Expression.ArrayIndex(param, index);

				var paramCastExp =
					Expression.Convert(paramAccessorExp, paramType);

				argsExp[i] = paramCastExp;
			}

			//make a NewExpression that calls the
			//ctor with the args we just created
			var newExp = Expression.New(ctor, argsExp);

			//create a lambda with the New
			//Expression as body and our param object[] as arg
			var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

			return (ObjectActivator<T>)lambda.Compile();
		}

	}
}
