using UnityEngine;
using System.Collections.Generic;
using System;
using MovementEffects;

public class MessageBus : SingletonMono<MessageBus>
{
	private Dictionary<MessageBusType, List<Action<Message>>> listeners = new Dictionary<MessageBusType, List<Action<Message>>>(50);
	private Timing timer;
	private Queue<Message> normalMSGs = new Queue<Message>();
	private float lastBreakTime = 0;
	private float MAX_FRAME_TIME = 1f / 60f;
	public static bool isInitialize = false;

	public void Initialize()
	{
		timer = gameObject.AddComponent<Timing>();
		normalMSGs = new Queue<Message>(25);
		//timer.RunCoroutineOnInstance(ProcessMessage(), Segment.Update);
		isInitialize = true;
		isInit = true;
	}

	private bool isInit;
	private void Update()
	{
		if (this.isInit == false)
		{
			return;
		}

		if (normalMSGs != null && this.normalMSGs.Count == 0)
		{
			return;
		}

		this.lastBreakTime = Time.realtimeSinceStartup;
		while (normalMSGs != null && this.normalMSGs.Count > 0)
		{
			float deltaBreakTime = Time.realtimeSinceStartup - lastBreakTime;
			if (deltaBreakTime >= this.MAX_FRAME_TIME)
			{
				//Debug.LogWarning("<b><color=#ffcc11>MessageBus - Break frame time: </color></b>" + deltaBreakTime);
				break;
			}

			DispatchMessage(normalMSGs.Dequeue());
		}
	}

	public void Subscribe(MessageBusType type, Action<Message> handler)
	{
		if (listeners.ContainsKey(type))
		{
			if (listeners[type] == null)
			{
				listeners[type] = new List<Action<Message>>(20);
			}
		}
		else
		{
			listeners.Add(type, new List<Action<Message>>(20));
		}

		listeners[type].Add(handler);
	}

	public void Unsubscribe(MessageBusType type, Action<Message> handler)
	{
		if (listeners.ContainsKey(type))
		{
			if (listeners[type] != null)
			{
				listeners[type].Remove(handler);
			}
		}
	}

	/// <summary>
	/// Send a message to all subscribers listening to this certain message type
	/// </summary>
	/// <param name="message"></param>
	public void SendMessage(Message message, bool immidiately = false)
	{
		if (immidiately)
		{
			DispatchMessage(message);
		}
		else
		{
			normalMSGs.Enqueue(message);
		}
	}

	private void DispatchMessage(Message message)
	{
		List<Action<Message>> listHandlers = null;
		if (listeners.TryGetValue(message.messageType, out listHandlers))
		{
			List<Action<Message>> cloneList = new List<Action<Message>>(listHandlers);

			for (int i = cloneList.Count - 1; i >= 0; i--)
			{
				if (cloneList[i] != null)
				{
					cloneList[i](message);
				}
			}
		}
	}


	/// <summary>
	/// Push a message directly into processing. Note that this call is synchronous and is not in framerate control
	/// </summary>
	/// <param name="message"></param>
	public static void AnnouceHighPriorityMessage(Message message)
	{
		Instance.DispatchMessage(message);
	}

	/// <summary>
	/// Short hand for calling send message with normal priority (will be delay if the target framerate is not reached)
	/// </summary>
	public static void Annouce(Message message)
	{
		Instance.SendMessage(message);
	}
}

public enum MessageBusType
{
    NONE = 0,
    UPDATE_MONEY = 1,
}

