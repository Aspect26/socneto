using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Socneto.Domain.Services
{
    public class CsvService : ICsvService
    {
        private readonly string _delimiter = ",";
        
        public string GetCsv<T>(IEnumerable<T> data)
        {
            var itemType = data.GetType().GetGenericArguments()[0];
            var csvStringBuilder = new StringBuilder();

            var propertyNames = GetPropertyNames(itemType);
            csvStringBuilder.AppendLine(string.Join(_delimiter, propertyNames));
 
            foreach (var item in data)
            {
                var itemCsvRepresentation = GetObjectCsvRepresentation(item);
                csvStringBuilder.AppendLine(itemCsvRepresentation);
            }

            return csvStringBuilder.ToString();
        }

        private IEnumerable<string> GetPropertyNames(Type type)
        {
            return type.GetProperties().Where(pi => !pi.GetCustomAttributes<JsonIgnoreAttribute>(false).Any())
                .Select(GetDisplayNameFromNewtonsoftJsonAnnotations);
        }

        private string GetObjectCsvRepresentation<T>(T item)
        {
            var itemProperties = item.GetType().GetProperties()
                .Where(pi => !pi.GetCustomAttributes<JsonIgnoreAttribute>().Any())
                .Select(pi => pi.GetValue(item, null))
                .ToList();
 
            var objectCsvRepresentation = new StringBuilder();
 
            foreach (var itemProperty in itemProperties)
            {
                var itemPropertyValueString = GetObjectPropertyValue(itemProperty);
                objectCsvRepresentation.Append(itemPropertyValueString);
                if (itemProperty != itemProperties.Last())
                    objectCsvRepresentation.Append(_delimiter);
            }

            return objectCsvRepresentation.ToString();
        }

        private string GetObjectPropertyValue(object property)
        {
            if (property == null) return string.Empty;
            
            var itemPropertyValueString = property.ToString();
 
            if (itemPropertyValueString.Contains(_delimiter))
                itemPropertyValueString = $"\"{itemPropertyValueString}\"";
            if (itemPropertyValueString.Contains("\r"))
                itemPropertyValueString = itemPropertyValueString.Replace("\r", " ");
            if (itemPropertyValueString.Contains("\n"))
                itemPropertyValueString = itemPropertyValueString.Replace("\n", " ");

            return itemPropertyValueString;
        }
        
        private string GetDisplayNameFromNewtonsoftJsonAnnotations(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<JsonPropertyAttribute>(false)?.PropertyName is string value)
            {
                return value;
            }
 
            return pi.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? pi.Name;
        }
    }
    
}
