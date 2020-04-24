using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ladybug.ECS
{
	public class EntitySystem
	{
		private ulong _nextEntityID = 0;
		private Dictionary<ulong, Entity> _entityList = new Dictionary<ulong, Entity>();
		private Dictionary<Type, List<Component>> _componentList;
		private List<IDrawableComponent> _drawableComponents = new List<IDrawableComponent>();

		public EntitySystem(){}

		public EntitySystem(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice;
		}

		public GraphicsDevice GraphicsDevice { get; set; }

		public Entity FindEntity(string name)
		=> _entityList.Where((item => item.Value.Name == name)).FirstOrDefault().Value;

		public void InitializeComponents()
		{
			foreach (var ctype in _componentList)
			{
				foreach (var component in ctype.Value)
				{
					component.Initialize();
				}
			}
		}

		public void PreUpdate(GameTime gameTime)
		{
			foreach (var ctype in _componentList)
			{
				foreach (var component in ctype.Value)
				{
					if (component.Entity.Active && component.Active)
					{
						component.PreUpdate(gameTime);
					}
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			foreach (var ctype in _componentList)
			{
				foreach (var component in ctype.Value)
				{
					if (component.Entity.Active && component.Active)
					{
						component.Update(gameTime);
					}
				}
			}
		}

		public void PostUpdate(GameTime gameTime)
		{
			foreach (var ctype in _componentList)
			{
				foreach (var component in ctype.Value)
				{
					if (component.Entity.Active && component.Active)
					{
						component.PostUpdate(gameTime);
					}
				}
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (var c in _drawableComponents)
			{
				if (c.Visible)
				{
					c.Draw(gameTime, spriteBatch);
				}
			}
		}

		internal ulong RegisterEntity(Entity e)
		{
			var eID = RequestID();
			_entityList.Add(eID, e);
			return eID;
		}

		internal void RegisterComponent(Component c)
		{
			var t = c.GetType();
			if (_componentList == null)
			{
				_componentList = new Dictionary<Type, List<Component>>();
			}
			if (!(_componentList.ContainsKey(t)))
			{
				_componentList[t] = new List<Component>();
			}
			_componentList[t].Add(c);

			IDrawableComponent drawableComponent = c as IDrawableComponent;

			if (drawableComponent != null)
			{
				_drawableComponents.Add(drawableComponent);
				SortDrawableComponents();
			}
		}

		internal void DeregisterComponent(Component c)
		{
			var t = c.GetType();
			_componentList[t].Remove(c);

			IDrawableComponent drawableComponent = c as IDrawableComponent;

			if (drawableComponent != null)
			{
				_drawableComponents.Remove(drawableComponent);
			}
		}

		private void DeregisterEntity(Entity e)
		{
			_entityList.Remove((ulong)e.ID);
		}

		private ulong RequestID()
		{
			ulong id = _nextEntityID;
			_nextEntityID++;

			return id;
		}

		private void SortDrawableComponents()
		{
			_drawableComponents.Sort(delegate (IDrawableComponent x, IDrawableComponent y)
				{
					int res = 0;
					if (x.DrawPriority > y.DrawPriority) res = 1;
					if (x.DrawPriority < y.DrawPriority) res = -1;
					if (x.DrawPriority == y.DrawPriority) res = 0;
					return res;
				}
			);
		}
	}
}