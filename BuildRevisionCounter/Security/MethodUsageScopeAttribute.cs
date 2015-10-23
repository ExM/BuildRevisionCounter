using System;
using System.Reflection;

namespace BuildRevisionCounter.Security
{
	public class MethodUsageScopeAttribute: Attribute
	{
		public string MainModuleName { get; set; }

		public static void ControlScope(MemberInfo methodBase)
		{
			var att = (MethodUsageScopeAttribute)GetCustomAttribute(methodBase, typeof(MethodUsageScopeAttribute));
			if (att == null || att.MainModuleName == null)
				return;
			var allowNames = att.MainModuleName.ToLower().Split(',');
			var moduleName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName.ToLower();
			foreach (var name in allowNames)
			{
				if (name == moduleName)
					return;
			}
			throw new InvalidOperationException();
		}
	}
}