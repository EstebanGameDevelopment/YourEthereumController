using System.Collections.Generic;
using UnityEngine;
using YourCommonTools;

namespace YourEthereumController
{
    public delegate void EthereumEventHandler(string _nameEvent, params object[] _list);

	/******************************************
	 * 
	 * BasicEventController
	 * 
	 * Class used to dispatch events through all the system
	 * 
	 * @author Esteban Gallardo
	 */
	public class EthereumEventController : MonoBehaviour
	{
		public event EthereumEventHandler EthereumEvent;

		// ----------------------------------------------
		// SINGLETON
		// ----------------------------------------------	
		private static EthereumEventController _instance;

		public static EthereumEventController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType(typeof(EthereumEventController)) as EthereumEventController;
					if (!_instance)
					{
						GameObject container = new GameObject();
						string finalSingletonName = "^EthereumEventController^";
#if !ENABLE_MY_OFUSCATION || UNITY_EDITOR
						finalSingletonName = finalSingletonName.Replace("^", "");
#endif
						container.name = finalSingletonName;
						_instance = container.AddComponent(typeof(EthereumEventController)) as EthereumEventController;
						DontDestroyOnLoad(_instance);
					}
				}
				return _instance;
			}
		}

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private List<TimedEventData> m_listEvents = new List<TimedEventData>();

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		private EthereumEventController()
		{
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public void Destroy()
		{
			Destroy(_instance.gameObject);
			_instance = null;
		}

		// -------------------------------------------
		/* 
		 * Will dispatch an event
		 */
		public void DispatchEthereumEvent(string _nameEvent, params object[] _list)
		{
			if (EthereumEvent != null) EthereumEvent(_nameEvent, _list);
		}

		// -------------------------------------------
		/* 
		 * Will add a new delayed event to the queue
		 */
		public void DelayEthereumEvent(string _nameEvent, float _time, params object[] _list)
		{
			m_listEvents.Add(new TimedEventData(_nameEvent, _time, _list));
		}

		// -------------------------------------------
		/* 
		 * Clone a delayed event
		 */
		public void DelayEthereumEvent(TimedEventData _timeEvent)
		{
			m_listEvents.Add(new TimedEventData(_timeEvent.NameEvent, _timeEvent.Time, _timeEvent.List));
		}

		// -------------------------------------------
		/* 
		 * Will process the queue of delayed events 
		 */
		void Update()
		{
			// DELAYED EVENTS
			for (int i = 0; i < m_listEvents.Count; i++)
			{
				TimedEventData eventData = m_listEvents[i];
				eventData.Time -= Time.deltaTime;
				if (eventData.Time <= 0)
				{
					EthereumEvent(eventData.NameEvent, eventData.List);
					eventData.Destroy();
					m_listEvents.RemoveAt(i);
					break;
				}
			}
		}
	}
}
