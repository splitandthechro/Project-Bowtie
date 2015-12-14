using System;
using nginz.Common;

namespace ProjectBowtie
{
	public class EnemyConfigurationHandler : AssetHandler<EnemyConfiguration>
	{
		public EnemyConfigurationHandler (ContentManager content)
			: base (content, string.Empty) { }

		public override EnemyConfiguration Load (string assetName, params object[] args) {
			var filename = assetName.EndsWith (".json")
				? assetName
				: string.Format ("{0}.json", assetName);
			return EnemyConfiguration.Load (filename);
		}

		public override void Save (EnemyConfiguration asset, string assetPath) {
			var filename = assetPath.EndsWith (".json")
				? assetPath
				: string.Format ("{0}.json", assetPath);
			asset.Save (filename);
		}
	}
}

