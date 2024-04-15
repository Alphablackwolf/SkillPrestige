using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SkillPrestige.Logging;

namespace SkillPrestige.Extensions
{
    /// <summary>
    /// Reflection Extension methods created and used for SkillPrestige.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// gets the field from an object through reflection, even if it is a private field.
        /// </summary>
        /// <typeparam name="T">The type that contains the parameter member</typeparam>
        /// <param name="instance">The instance of the type you wish to get the field from.</param>
        /// <param name="fieldName">The name of the field you wish to retrieve.</param>
        /// <returns>Returns null if no field member found.</returns>
        public static object GetInstanceField<T>(this T instance, string fieldName)
        {
            //Logger.LogVerbose($"Obtaining instance field {fieldName} on object of type {instance.GetType().FullName}");
            const BindingFlags bindingAttributes = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var memberInfo = instance.GetType().GetField(fieldName, bindingAttributes);
            return memberInfo?.GetValue(instance);
        }

        /// <summary>
        /// sets the field from an object through reflection, even if it is a private field.
        /// </summary>
        /// <typeparam name="T">The type that contains the parameter member</typeparam>
        /// <typeparam name="TMember">The type of the parameter member</typeparam>
        /// <param name="instance">The instance of the type you wish to set the field value of.</param>
        /// <param name="fieldName">>The name of the field you wish to set.</param>
        /// <param name="value">The value you wish to set the field to.</param>
        // ReSharper disable once UnusedMember.Global
        public static void SetInstanceField<T, TMember>(this T instance, string fieldName, TMember value)
        {
            //Logger.LogVerbose($"Obtaining instance field {fieldName} on object of type {instance.GetType().FullName}");
            const BindingFlags bindingAttributes = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var memberInfo = instance.GetType().GetField(fieldName, bindingAttributes);
            memberInfo?.SetValue(instance, value);
        }

        /// <summary>
        /// Invokes and returns the value of a static function, even if it is a private member.
        /// </summary>
        /// <typeparam name="TReturn">The type of the returned function.</typeparam>
        /// <param name="type">The type that contains the static function.</param>
        /// <param name="functionName">The name of the static function.</param>
        /// <param name="arguments">The arguments passed to the static function.</param>
        /// <returns></returns>
        public static TReturn InvokeStaticFunction<TReturn>(this Type type, string functionName, params object[] arguments)
        {
            try
            {
                const BindingFlags bindingAttributes = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                var method = type.GetMethod(functionName, bindingAttributes);
                if (method == null) return default(TReturn);
                var result = method.Invoke(null, arguments);
                // ReSharper disable once MergeConditionalExpression - a conditional merge would make this nonviable to work with both reference and value types.
                return result is TReturn ? (TReturn)result : default(TReturn);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return default(TReturn);
            }
        }

        /// <summary>
        /// gets the field from an object's base through reflection, even if it is a private field.
        /// </summary>
        /// <typeparam name="T">The type that is derived from the type that contains the parameter member</typeparam>
        /// <param name="instance">The instance of the derived type you wish to get the field from the base of.</param>
        /// <param name="fieldName">The name of the field you wish to retrieve.</param>
        /// <returns>Returns null if no field member found.</returns>
        public static object GetInstanceFieldOfBase<T>(this T instance, string fieldName)
        {
            const BindingFlags bindingAttributes = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var memberInfo = instance.GetType().BaseType?.GetField(fieldName, bindingAttributes);
            return memberInfo?.GetValue(instance);
        }

        /// <summary>
        /// sets the field of a base class of an object through reflection, even if it is a private field.
        /// </summary>
        /// <typeparam name="T">The type that directly inherits from the type contains the parameter member</typeparam>
        /// <typeparam name="TMember">The type of the parameter member</typeparam>
        /// <param name="instance">The instance of the type you wish to set the field value of.</param>
        /// <param name="fieldName">>The name of the field you wish to set.</param>
        /// <param name="value">The value you wish to set the field to.</param>
        public static void SetInstanceFieldOfBase<T, TMember>(this T instance, string fieldName, TMember value)
        {
            //Logger.LogVerbose($"Obtaining instance field {fieldName} on object of type {instance.GetType().FullName}");
            const BindingFlags bindingAttributes = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var memberInfo = instance.GetType().BaseType?.GetField(fieldName, bindingAttributes);
            memberInfo?.SetValue(instance, value);
        }

        public static IEnumerable<Assembly> GetNonSystemAssemblies(this AppDomain appDomain)
        {
            return appDomain.GetAssemblies().Where(x => !x.FullName.StartsWithOneOf("mscorlib", "System", "Microsoft", "Windows", "Newtonsoft"));
        }


        /// <summary>
        /// Gets types from an assembly as long as those types can safely be retrieved.
        /// </summary>
        /// <param name="assembly">Assembly you wish to obtain types from.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
        {
            try
            {
                Logger.LogTrace($"Attempting to obtain types of assembly {assembly.FullName} safely...");
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                Logger.LogTrace($"Failed to load a type from assembly {assembly.FullName}. details: {Environment.NewLine} {exception}");
                return exception.Types.Where(x => x != null);
            }
        }

        /// <summary>
        /// Creates a hook into a method, allowing further manipulation.
        /// </summary>
        /// <param name="typeToHookInto">The type that contains the method you wish to hook into.</param>
        /// <param name="methodNameToHookTo">The method name to hook into for further manipulation.</param>
        /// <returns></returns>
        public static IMethodHook HookToMethod(this Type typeToHookInto, string methodNameToHookTo)
        {
            return (IMethodHook)Activator.CreateInstance(typeof(MethodHook<>).MakeGenericType(typeToHookInto), args: methodNameToHookTo);
        }

        public static bool MethodSignaturesMatch(this MethodInfo method, MethodInfo methodToCompare)
        {
            if (method.ReturnType != methodToCompare.ReturnType) return false;
            var parameterTypes = method.GetParameters();
            var parameterTypesToCompare = methodToCompare.GetParameters();
            if (parameterTypes.Length != parameterTypesToCompare.Length) return false;
            return !parameterTypes.Where((t, i) => t.ParameterType != parameterTypesToCompare[i].ParameterType).Any();
        }
    }

    public interface IMethodHook
    {
        /// <summary>
        /// Replaces the hooked method with a same-named method in the given type.
        /// </summary>
        /// <typeparam name="T">The type containing the method that will replace the hooked method. The method names must match.</typeparam>
        /// <returns></returns>
        bool ReplaceUsing<T>();

        /// <summary>
        /// Replaces the hooked method with a method of the given name.
        /// </summary>
        /// <typeparam name="T">The type that contains the method that will be injected.</typeparam>
        /// <param name="replacementMethodName">The injected method name.</param>
        /// <returns></returns>
        bool ReplaceWith<T>(string replacementMethodName);
    }

    public class MethodHook<T> : IMethodHook
    {
        private readonly MethodInfo _method;

        private const BindingFlags AllBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        public MethodHook(string methodName)
        {
            Logger.LogInformation($"Hooking to method {methodName} in class: {typeof(T).Name}");
            _method = typeof(T).GetMethod(methodName, AllBindingFlags);
            Logger.LogInformation($"Method {(_method == null ? "not " : string.Empty)}found.");
            Logger.LogInformation($"method is same as base: {_method?.GetBaseDefinition() == _method}");
        }
        public bool ReplaceUsing<TReplace>()
        {
            Logger.LogInformation($"replacing {_method.Name} method in class: {typeof(TReplace).Name}");
            var methodToInject = typeof(TReplace).GetMethod(_method.Name);
            Logger.LogInformation($"Method {(methodToInject == null ? "not " : string.Empty)}found.");
            return Replace(_method, methodToInject);
        }

        public bool ReplaceWith<TReplace>(string replacementMethodName)
        {
            Logger.LogInformation($"replacing {replacementMethodName} method in class: {typeof(TReplace).Name}");
            var methodToInject = typeof(TReplace).GetMethod(replacementMethodName, AllBindingFlags);
            Logger.LogInformation($"Method {(methodToInject == null ? "not " : string.Empty)}found.");
            return Replace(_method, methodToInject);
        }

        /// <summary>
        /// Replaces one method with another.
        /// </summary>
        /// <returns></returns>
        private static bool Replace(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            if (!methodToReplace.MethodSignaturesMatch(methodToInject))
            {
                Logger.LogError($"{methodToReplace.Name} could not be replaced with {methodToInject.Name} due to unmatching signatures.");
                return false;
            }
            Logger.LogInformation("Replacing Method...");
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            if (methodToReplace.IsVirtual)
            {
                ReplaceVirtualInner(methodToReplace, methodToInject);
            }
            else
            {
                ReplaceInner(methodToReplace, methodToInject);
            }

            Logger.LogInformation("ReplaceUsing Successful.");
            return true;
        }

        private static void ReplaceInner(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    var injectedMethodPointer = (uint*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    var targetMethodPointer = (uint*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
                    Logger.LogInformation($"(x86)Source pointer:{*injectedMethodPointer} | Target pointer: {*targetMethodPointer}");
                    *targetMethodPointer = *injectedMethodPointer;
                    Logger.LogInformation($"(x86)New Target pointer: {*targetMethodPointer}");
                }
                else
                {

                    var injectedMethodPointer = (ulong*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    var targetMethodPointer = (ulong*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
                    Logger.LogInformation($"(x64)Source pointer:{*injectedMethodPointer} | Target pointer: {*targetMethodPointer}");
                    *targetMethodPointer = *injectedMethodPointer;
                    Logger.LogInformation($"(x64)New Target pointer: {*targetMethodPointer}");
                }
            }
        }

        private static void ReplaceVirtualInner(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            if (methodToReplace.DeclaringType == null) throw new ArgumentException("Method cannot be dynamic", nameof(methodToReplace));
            Logger.LogInformation("Replacing virtual method...");
            unsafe
            {
                var targetMethodBasePointer = (ulong*)methodToReplace.MethodHandle.Value.ToPointer();
                var index = (int)((*targetMethodBasePointer >> 32) & 0xFF);
                if (IntPtr.Size == 4)
                {
                    
                    var classStart = (uint*)methodToReplace.DeclaringType.TypeHandle.Value.ToPointer() + 10;
                    classStart = (uint*) *classStart;
                    var targetMethodPointer = classStart + index;
                    var injectedMethodPointer = (uint*) methodToInject.MethodHandle.Value.ToPointer() + 2;
                    Logger.LogInformation($"(x86)Source pointer:{*injectedMethodPointer} | Target pointer: {*targetMethodPointer}");
                    *targetMethodPointer = *injectedMethodPointer;
                    Logger.LogInformation($"(x86)New Target pointer: {*targetMethodPointer}");
                }
                else
                {
                    var classStart = (ulong*) methodToReplace.DeclaringType.TypeHandle.Value.ToPointer() + 8;
                    classStart = (ulong*) *classStart;
                    var targetMethodPointer = classStart + index;
                    var injectedMethodPointer = (ulong*)methodToInject.MethodHandle.Value.ToPointer()
                        + 1;
                    Logger.LogInformation($"(x64)Source pointer:{*injectedMethodPointer} | Target pointer: {*targetMethodPointer}");
                    *targetMethodPointer = *injectedMethodPointer;
                    Logger.LogInformation($"(x64)New Target pointer: {*targetMethodPointer}");
                }
            }
        }
    }
}
