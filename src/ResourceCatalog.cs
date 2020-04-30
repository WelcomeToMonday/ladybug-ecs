using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ladybug.ECS
{
	public class ResourceCatalog
	{
		public Dictionary<Type, Dictionary<string, object>> Catalog { get; private set; } = new Dictionary<Type, Dictionary<string, object>>();

		private ContentManager Content;

		public ResourceCatalog(ContentManager contentManager)
		{
			Content = contentManager;
		}

		public void LoadResource<T>(string identifier, string source)
		{
			if (!Catalog.ContainsKey(typeof(T)))
			{
				Catalog[typeof(T)] = new Dictionary<string, object>();
			}

			if (!Catalog[typeof(T)].ContainsKey(identifier))
			{
				var resource = Content.Load<T>(source);

				Catalog[typeof(T)][identifier] = resource as object;
			}
		}

		public T GetResource<T>(string name)
		{
			T res = default(T);

			try
			{
				res = (T)Catalog[typeof(T)][name];
			}
			catch (KeyNotFoundException)
			{
				res = default(T);
			}

			return res;
		}
	}
}