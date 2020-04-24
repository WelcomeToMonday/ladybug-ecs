using System; //temp

using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ladybug.ECS
{
	public sealed class Entity : IXmlSerializable
	{
		public static Dictionary<int, string> Tags;

		private List<Component> m_components;

		public Entity(EntitySystem entitySystem, string name = null)
		{
			System = entitySystem;
			ID = System.RegisterEntity(this);
			if (name != null)
			{
				Name = name;
			}
		}

		public EntitySystem System { get; private set; }

		public string Name { get; private set; }

		public ulong? ID { get; private set; } = null;

		public bool Active { get; set; } = true;

		private List<Component> Components
		{
			get
			{
				if (m_components == null)
				{
					m_components = new List<Component>();
				}
				return m_components;
			}
			set => m_components = value;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public T AddComponent<T>(string name = null) where T : Component, new()
		{
			var comp = new T();
			comp.SetEntity(this);
			if (name != null)
			{
				comp.SetName(name);
			}
			Components.Add(comp);
			System.RegisterComponent(comp);
			return comp;
		}

		public Component AddComponent(Component c, string name = null)
		{
			c.SetEntity(this);
			Components.Add(c);
			if (name != null)
			{
				c.SetName(name);
			}
			System.RegisterComponent(c);
			return c;
		}

		public void RemoveComponent(Component c)
		{
			System.DeregisterComponent(c);
			Components.Remove(c);
		}

		public T GetComponent<T>() where T : Component
		=> Components.OfType<T>().Where(item => item.GetType() == typeof(T)).FirstOrDefault();

		public T GetComponent<T>(string name) where T : Component
		=> Components.OfType<T>().Where(item => (item.GetType() == typeof(T) && item.Name == name)).FirstOrDefault();

		// XML Serialization

		public void SaveToXml(string filePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
			using (XmlWriter writer = XmlWriter.Create(filePath, settings))
			{
				WriteXml(writer);
			}
		}

		public static Entity LoadFromXml(EntitySystem system, string filepath)
		{
			var e = new Entity(system);
			using (XmlReader reader = XmlReader.Create(filepath))
			{
				e.ReadXml(reader);
			}
			return e;
		}

		/// <summary>
		/// Writes the Entity to an XML file
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// Please use SaveToXml instead unless
		/// you need to provide your own XmlWriter
		/// </remarks>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Entity");
			writer.WriteAttributeString("id", ID.ToString());
			writer.WriteAttributeString("name", Name);
			if (Components != null && Components.Count > 0)
			{
				foreach (var c in Components)
				{
					c.WriteXml(writer);
				}
			}
			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					Type t = Type.GetType(reader.Name);
					if (t != null && t.IsSubclassOf(typeof(Component)))
					{
						var newComponent = (Component)Activator.CreateInstance(t);
						using (var sub = reader.ReadSubtree())
						{
							newComponent.ReadXml(sub);
						}
						AddComponent(newComponent);
					}
				}
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}
	}
}