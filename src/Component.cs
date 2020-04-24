using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;

namespace Ladybug.ECS
{
	public abstract class Component : IXmlSerializable
	{	
		public Component(Entity entity = null, string Name = null)
		{
			SetEntity(entity);
		}

		public Entity Entity { get; private set; }

		public string Name { get; protected set; }

		public bool Active { get; protected set; } = true;

		public void SetName(string name)
		{
			Name = name;
		}

		public void SetActive(bool active)
		{
			Active = active;
		}

		public virtual void Initialize(){}
		
		public virtual void PreUpdate(GameTime gameTime){}

		public virtual void Update(GameTime gameTime){}
		
		public virtual void PostUpdate(GameTime gameTime){}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement($"{ToString()}");
			writer.WriteEndElement();
		}
		
		public virtual void ReadXml(XmlReader reader){}
		
		public virtual XmlSchema GetSchema() => null;

		public override string ToString() => $"{GetType()}";

		internal void SetEntity(Entity entity)
		{
			Entity = entity;
		}
	}
}