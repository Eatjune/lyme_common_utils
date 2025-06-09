using System;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 类，对象，反射相关
	/// </summary>
	public static class ClassUtils {
		/// <summary>
		/// 根据类型字符串获取类型
		/// </summary>
		/// <param name="className">类名</param>
		public static Type GetType(string className) {
			Type type = Type.GetType(className);
			if (type == null) {
				type = Type.GetType($"GamePlay.{className}");
				if (type == null) {
					Debug.LogError($"{className} is not exist!");
					return null;
				}
			}

			return type;
		}

		/// <summary>
		/// 反射实例化类
		/// </summary>
		/// <param name="className">类名</param>
		public static object InstantiateClassByString(string className, params object[] args) {
			Type type = GetType(className);
			if (type == null) {
				Debug.LogError($"{className} is not exist!");
				return null;
			}

			var obj = Activator.CreateInstance(type, args);
			return obj;
		}

		/// <summary>
		/// 调用私有方法,如果没有方法则不调用
		/// </summary>
		/// <param name="obj">实例对象</param>
		/// <param name="method">方法名</param>
		/// <param name="args">参数</param>
		public static void CallMethod(object obj, string method, params object[] args) {
			Type type = obj.GetType();
			var _method = type.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			if (_method == null) return;
			_method.Invoke(obj, args);
		}

		private const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		//反射参考: https://blog.csdn.net/qq_38601621/article/details/103578732

		/// <summary>
		/// 获取对象的成员信息
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="fieldName">所在域</param>
		/// <returns></returns>
		public static object GetMember(object obj, string fieldName) {
			Type type = obj.GetType();
			var _filedInfo = type.GetMember(fieldName);
			if (_filedInfo.Length == 0) return null;
			var _filed = type.InvokeMember(fieldName, flags | BindingFlags.GetField, null, obj, null);
			return _filed;
		}

		/// <summary>
		/// 获取对象debug信息，包括成员，属性，字段
		/// </summary>
		/// <param name="obj">对象</param>
		public static void DebugObject(object obj) {
			var type = obj.GetType();
			var members = type.GetMembers();
			var propertys = type.GetProperties();
			var fields = type.GetFields();
			foreach (MemberInfo member in members) {
				Debug.Log("成员名是" + member.Name + "对应的类是" + member.DeclaringType.ToString());
			}

			foreach (PropertyInfo proerty in propertys) {
				Debug.Log("属性是" + proerty.Name + "对应的类是" + proerty.DeclaringType.ToString());
			}

			foreach (FieldInfo field in fields) {
				Debug.Log("字段是" + field.Name + "对应的类是" + field.DeclaringType.ToString());
			}
		}
	}
}