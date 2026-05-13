// (c) 2023 W2 Co.,Ltd.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using w2.Common.Helper.Attribute;

namespace w2.BBS.Front.Codes.Binding
{
	/// <summary>
	/// エイリアスバインダ
	/// </summary>
	public sealed class AliasModelBinder : DefaultModelBinder
	{
		/// <inheritdoc />
		protected override object GetPropertyValue(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext,
			PropertyDescriptor propertyDescriptor,
			IModelBinder propertyBinder)
		{
			// バインドするときに空文字をnullに変更しない
			bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
			var result = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
			return result;
		}

		/// <inheritdoc />
		protected override PropertyDescriptorCollection GetModelProperties(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext)
		{
			var toReturn = base.GetModelProperties(controllerContext, bindingContext);
			var additional = new List<PropertyDescriptor>();

			foreach (var p in GetTypeDescriptor(controllerContext, bindingContext).GetProperties().Cast<PropertyDescriptor>())
			{
				foreach (var attr in p.Attributes.OfType<BindAliasAttribute>())
				{
					additional.Add(new AliasedPropertyDescriptor(attr.Alias, p));

					if (bindingContext.PropertyMetadata.ContainsKey(p.Name))
					{
						bindingContext.PropertyMetadata.Add(attr.Alias, bindingContext.PropertyMetadata[p.Name]);
					}
				}
			}

			return new PropertyDescriptorCollection(toReturn.Cast<PropertyDescriptor>().Concat(additional).ToArray());
		}

		/// <inheritdoc />
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType.IsEnum)
			{
				var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
				if (value != null)
				{
					// バインドされた値はEnumApiNameAttributeに指定された値である前提で、
					// EnumApiNameAttributeで指定された列挙子に変換している
					// public enum Sample{
					//   [EnumApiName("A")]
					//   Hoge,
					//   [EnumApiName("B")]
					//   Foo
					// }
					// 上の列挙体がリクエストで指定されていた場合、Aという文字列が来たらSample.Hogeに変換してるってこと
					var attemptedValue = value.AttemptedValue;
					foreach (var item in Enum.GetValues(bindingContext.ModelType).Cast<Enum>())
					{
						var field = bindingContext.ModelType.GetField(item.ToString());
						var attribute = field.GetCustomAttributes(typeof(EnumApiNameAttribute), false).FirstOrDefault() as EnumApiNameAttribute;
						if (attribute != null && attribute.Value == attemptedValue)
						{
							return item;
						}

						if (int.TryParse(attemptedValue, out var intValue)
							&& intValue == Convert.ToInt32(item))
						{
							return item;
						}
					}

					throw new Exception($"列挙体をバインドできませんでした。type={bindingContext.ModelType} attempted_value={attemptedValue}");
				}
			}

			return base.BindModel(controllerContext, bindingContext);
		}
	}
}
