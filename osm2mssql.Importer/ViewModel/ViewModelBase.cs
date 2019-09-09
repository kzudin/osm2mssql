using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace osm2mssql.Importer.ViewModel
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged([CallerMemberName]string property = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
		
		protected void SaveModelToFile<T>(string filePath, T Model)
		{
			try
			{
				var ser = new XmlSerializer(typeof(T));
				using (var file = File.Create(filePath))
				{
					ser.Serialize(file, Model);
				}
			}
			catch
			{

			}
		}

		protected T LoadModelFromFile<T>(string filePath)
		{
			try
			{
				var ser = new XmlSerializer(typeof(T));
				using (var file = File.Open(filePath, FileMode.Open))
				{
					return (T)ser.Deserialize(file);
				}
			}
			catch
			{
				return Activator.CreateInstance<T>();
			}
		}
	}
}