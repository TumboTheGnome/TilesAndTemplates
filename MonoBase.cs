using UnityEngine;
using System.Collections;

public delegate void MonoBaseEvent();
public delegate void MonoBaseTriggerEvent(Collider c);
public delegate void MonoBaseCollisionEvent(Collision c);
public class MonoBase : MonoBehaviour {

	public event MonoBaseEvent AwakeEvent; 
	public event MonoBaseEvent StartEvent; 
	public event MonoBaseEvent UpdateEvent;
	public event MonoBaseEvent FixedUpdateEvent;
	public event MonoBaseTriggerEvent TriggerEnterEvent;
	public event MonoBaseTriggerEvent TriggerStayEvent;
	public event MonoBaseTriggerEvent TriggerExitEvent;
	public event MonoBaseCollisionEvent CollisionEnterEvent;
	public event MonoBaseCollisionEvent CollisionStayEvent;
	public event MonoBaseCollisionEvent CollisionExitEvent;

	void Awake()
	{
		if (this.AwakeEvent != null) {
			this.AwakeEvent ();
		}
	}

	void Start()
	{
		if (this.StartEvent != null) {
			this.StartEvent();
		}
	}

	void Update()
	{
		if (this.UpdateEvent != null) {
			this.UpdateEvent();
		}
	}

	void FixedUpdate()
	{
		if (this.FixedUpdateEvent != null) {
			this.FixedUpdateEvent();
		}
	}


	void OnTriggerEnter(Collider c)
	{
		if (this.TriggerEnterEvent != null) {
			this.TriggerEnterEvent(c);
		}
	}

	void OnTriggerStay(Collider c)
	{
		if (this.TriggerStayEvent != null) {
			this.TriggerStayEvent(c);
		}
	}

	void OnTriggerExit(Collider c)
	{
		if (this.TriggerExitEvent != null) {
			this.TriggerExitEvent(c);
		}
	}

	void OnCollisionEnter(Collision c)
	{
		if (this.CollisionEnterEvent != null) {
			this.CollisionEnterEvent(c);
		}
	}

	void OnCollisionStay(Collision c)
	{
		if (this.CollisionStayEvent != null) {
			this.CollisionStayEvent(c);
		}
	}

	void OnCollisionExit(Collision c)
	{
		if (this.CollisionExitEvent != null) {
			this.CollisionExitEvent(c);
		}
	}
}
