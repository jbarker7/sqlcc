using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SQLCC.Core.Interfaces;

namespace SQLCC
{
   public class AssemblyLoader
   {
      private readonly string _currentPath;

      public AssemblyLoader()
      {
         _currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      }

      private string[] ParseFullyQualifiedPath(string classNameAndAssemblyName)
      {
         var qualifiedName = classNameAndAssemblyName.Split(',');
         if (qualifiedName.Length != 2)
            throw new ArgumentOutOfRangeException("classNameAndAssemblyName", "classNameAndAssemblyName must be in format 'className, assemblyName' (assemblyName does not contain extension)");
         return qualifiedName.Select(p => p.Trim()).ToArray();
      }

      public Assembly Load(string classNameAndAssemblyName)
      {
         var type = Type.GetType(classNameAndAssemblyName, false);
         Assembly assembly;
         if (type == null)
         {
            var qualifiedName = ParseFullyQualifiedPath(classNameAndAssemblyName);
            assembly = Assembly.LoadFile(Path.Combine(_currentPath, qualifiedName[1] + ".dll"));
         }
         else
         {
            assembly = type.Assembly;
         }
         return assembly;
      }

      public Type GetTypeFromAssembly(string classNameAndAssemblyName)
      {
         var type = Type.GetType(classNameAndAssemblyName, false);
         if (type == null)
         {
            var qualifiedName = ParseFullyQualifiedPath(classNameAndAssemblyName);
            var assembly = this.Load(classNameAndAssemblyName);
            type = assembly.GetType(qualifiedName[0]);
         }
         return type;
      }

      public T CreateTypeFromAssembly<T>(string classNameAndAssemblyName, params object[] args)
         where T : IExtension
      {
         var type = this.GetTypeFromAssembly(classNameAndAssemblyName);
         return (T) Activator.CreateInstance(type, args);
      }

      public T CreateTypeFromAssembly<T>(string classNameAndAssemblyName, Dictionary<string, string> arguments)
         where T : IExtension
      {
         var type = this.GetTypeFromAssembly(classNameAndAssemblyName);
         var args = GetNamedArgumentArray((T) FormatterServices.GetUninitializedObject(type), arguments);
         return (T)Activator.CreateInstance(type, args);
      }

      public object[] GetNamedArgumentArray<T>(T extension, Dictionary<string, string> arguments)
         where T : IExtension
      {
         var args = arguments.Where(p => p.Key.StartsWith(extension.ArgumentNamespace + ".") && p.Key != extension.ArgumentNamespace + ".provider")
            .ToDictionary(p => p.Key, p => p.Value);

         var constructors = extension.GetType().GetConstructors();
         var providedSignature = string.Join(",", args.Select(p => p.Key).OrderBy(p => p));
         
         foreach (var constructor in constructors)
         {
            var parameters = constructor.GetParameters().Select(p => extension.ArgumentNamespace + "." + p.Name).ToArray();
            var orderedParams = string.Join(",", parameters.OrderBy(p => p));
            if (providedSignature != orderedParams) continue;
            var returnArgs = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
               returnArgs[i] = args[parameters[i]];
            }
            return returnArgs;
         }
         return new object[0];
      }
   }
}
