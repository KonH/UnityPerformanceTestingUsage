using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Tests {
	public sealed class ListTests {
		[Test, Performance]
		public void CreateAndFill() {
			Measure
				.Method(() => {
					var list = new List<string>();
					for (var i = 0; i < 100; i++) {
						list.Add("");
					}
				})
				.GC()
				.Run();
		}
	}
}