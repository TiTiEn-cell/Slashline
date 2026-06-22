using System.Collections.Generic;
using System;

namespace GFramework.Utils
{
    public class DoOnMainThread : SingletonMono<DoOnMainThread> {

		private static readonly Queue<Action> tasks = new Queue<Action>();

		public void Initial()
		{

		}
		void Update()
		{
			this.HandleTasks();
		}

		void HandleTasks()
		{
			while (tasks.Count > 0)
			{
				Action task = null;

				lock (tasks)
				{
					if (tasks.Count > 0)
					{
						task = tasks.Dequeue();
					}
				}

				task();
			}
		}

		public void QueueOnMainThread(Action task)
		{
			lock (tasks)
			{
				tasks.Enqueue(task);
			}
		}
	}

}
