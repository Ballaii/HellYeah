#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System;
using System.Linq;
using System.Reflection;
using Eliot.BehaviourEditor;
using System.Collections.Generic;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Responsible for the construction of delegates by methods names
	/// and for retrieving some reflective information from Agent's components.
	/// </summary>
	public static class EliotReflectionUtility
	{
        #region ReflectionToying

		/// <summary>
		/// Get an array of direct descendents if an arbitrary class T.
		/// </summary>
		/// <param name="T"></param>
		/// <returns></returns>
		public static List<Type> GetDirectExtentions(Type T){
			List<Type> types = new List<Type>();
			foreach (Type type in Assembly.GetAssembly(T).GetTypes())
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(T) && type.BaseType == (T))
					types.Add(type);
			return types;
		}
		
        /// <summary>
        /// Get an array of direct descendents if an arbitrary class T.
        /// </summary>
        /// <typeparam name="T">T can be any class.</typeparam>
        /// <returns>Returns an array of public nonstatic methods of arbitrary class.</returns>
        public static List<Type> GetDirectExtentions<T>(){
            List<Type> types = new List<Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes())
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)) && type.BaseType == (typeof(T)))
                    types.Add(type);
            return types;
        }
		
		public static List<Type> GetExtentions<T>(){
			List<Type> types = new List<Type>();
			foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes())
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)) )
					types.Add(type);
			return types;
		}
		
		/// <summary>
		/// Get an array of public nonstatic methods of arbitrary class.
		/// </summary>
		/// <typeparam name="T">T can be any class.</typeparam>
		/// <returns>Returns an array of public nonstatic methods of arbitrary class.</returns>
		public static string[] GetFunctions(Type T, bool sorted = true)
		{
			List<Type> types = GetDirectExtentions(T);
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach(var obj in types)
			{
				var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach(var method in localMethodsList)
					methods.Add(method);
			}
			var filtered = from method in methods
				where method.GetCustomAttributes(typeof(IncludeInBehaviour), false).Any()
				select method;
			var names = from name in filtered select name.Name;
			if (!sorted)
				return names.ToArray();
			var sortedNames = names.ToArray();
			Array.Sort(sortedNames, StringComparer.InvariantCulture);
			return sortedNames;
		}
		
		/// <summary>
		/// Get an array of public nonstatic methods of arbitrary class.
		/// </summary>
		/// <typeparam name="T">T can be any class.</typeparam>
		/// <returns>Returns an array of public nonstatic methods of arbitrary class.</returns>
        public static string[] GetFunctions<T>(bool sorted = true)
        {
            List<Type> types = GetDirectExtentions<T>();
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach(var obj in types)
            {
                var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach(var method in localMethodsList)
                    methods.Add(method);
            }
            var filtered = from method in methods
                           where method.GetCustomAttributes(typeof(IncludeInBehaviour), false).Any()
                           select method;
            var names = from name in filtered select name.Name;
            if (!sorted)
                return names.ToArray();
            var sortedNames = names.ToArray();
            Array.Sort(sortedNames, StringComparer.InvariantCulture);
            return sortedNames;
        }

		/// <summary>
		/// Get the first method in the given eliot interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static MethodInfo FirstMethod<T>()
		{
			List<Type> types = GetExtentions<T>();
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach(var obj in types)
			{
				var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach(var method in localMethodsList)
					methods.Add(method);
			}
			var filtered = from method in methods
				where method.GetCustomAttributes(typeof(IncludeInBehaviour), false).Any()
				select method;
			return filtered.First();
		}

        /// <summary>
        /// Get array of methods of a class corresponding to needed Condition Group
        /// </summary>
        /// <param name="conditionGroup"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string[] GetConditionStrings(ConditionGroup conditionGroup)
		{
			switch (conditionGroup)
			{
				case ConditionGroup.Resources: return GetFunctions<ResourcesConditionInterface>();
				case ConditionGroup.Perception: return GetFunctions<PerceptionConditionInterface>();
				case ConditionGroup.Motion: return GetFunctions<MotionConditionInterface>();
				case ConditionGroup.Inventory: return GetFunctions<InventoryConditionInterface>();
				case ConditionGroup.General: return GetFunctions<GeneralConditionInterface>();
                case ConditionGroup.Time: return new []{""};
                default: throw new ArgumentOutOfRangeException("conditionGroup", conditionGroup, null);
			}
		}

		/// <summary>
		/// Get array of methods of a class corresponding to needed index of Condition Group
		/// </summary>
		/// <param name="conditionGroupIndex"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static string[] GetConditionStrings(int conditionGroupIndex)
		{
			switch (conditionGroupIndex)
			{
				case 0: return GetConditionStrings(ConditionGroup.Resources);
				case 1: return GetConditionStrings(ConditionGroup.Perception);
				case 2: return GetConditionStrings(ConditionGroup.Motion);
				case 3: return GetConditionStrings(ConditionGroup.Inventory);
				case 4: return GetConditionStrings(ConditionGroup.General);
                case 5: return GetConditionStrings(ConditionGroup.Time);
                default: throw new ArgumentOutOfRangeException("conditionGroupIndex", conditionGroupIndex, null);
			}
		}
		
		/// <summary>
		/// Find a type given its name.
		/// </summary>
		/// <param name="methodName"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public static object GetTypeByMethodName<T>(string methodName)
        {
            List<Type> types = GetExtentions<T>();
            foreach (var obj in types)
            {
                var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in localMethodsList)
                    if (method.Name == methodName)
                        return obj;
            }
            return null;
        }
		
		/// <summary>
		/// Get a method info provided a method name and a class.
		/// </summary>
		/// <param name="T"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfoFromMethodName(Type T, string methodName)
		{
			List<Type> types = GetDirectExtentions(T);
			foreach (var obj in types)
			{
				var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (var method in localMethodsList)
					if (method.Name == methodName)
						return method;
			}
			return null;
		}

		/// <summary>
		/// Get the index of the given method in a group of methods grouped with an interface.
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static int[] GetIndexFromMethodInfo<T>(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				return new[]{-1, -1};
			}
			var indexClass = -1;
			var indexMethod = -1;
			
			var actionGroupsT = GetDirectExtentions<T>();
			for(int i = 0; i < actionGroupsT.Count; i++)
			{
				if (methodInfo.DeclaringType == actionGroupsT[i] || methodInfo.DeclaringType.IsSubclassOf(actionGroupsT[i]))
				{
					indexClass = i;
					break;
				}
			}
			
			if(indexClass == -1)
				return new[]{indexClass, indexMethod};
			
			// ReSharper disable once PossibleMistakenCallToGetType.2
			var options = GetFunctions(T: actionGroupsT[indexClass]);
			for(int i = 0; i < options.Length; i++)
			{
				if (options[i] == methodInfo.Name)
				{
					indexMethod = i;
					break;
				}
			}

			return new[]{indexClass, indexMethod};
		}

		#endregion
	}
}