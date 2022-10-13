using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using Spine.Unity;

using Array = System.Array;
using Random = System.Random;
using M7.Settings;

namespace M7.GameRuntime
{
	public class ExpressionController : MonoBehaviour
	{
		[SerializeField] private float m_Timeout = 2f;
		[SerializeField] private float m_AnimationDuration = 1f;

		// [SpineAnimation]
		// [SerializeField] private string m_MainAnim = null;

		// [SpineAnimation]
		// [SerializeField] private string[] m_ExpressionAnim = null;

		//[SerializeField] private SkeletonAnimation m_SkeletonAnim = null;

		private float m_Timer = 0.0f;
		private bool m_IsInteractable = false;
		private string m_MainAnim = null;
		private string[] m_ExpressionAnim = null;
		private Stack<int> m_ExpressionIndex = new Stack<int>();

		/// <summary>
		/// Listen to a specific OnClick event given through the parameter in order to play expressions.
		/// </summary>
		/// <param name="button">The button that the facial expression controller will listen to.</param>
		public void ListenToButton(UnityEngine.UI.Button button)
		{
			button.onClick.AddListener(OnButtonPressed);
		}

		public void Awake()
		{
			m_Timer = m_Timeout;
		}

		public void Start()
		{
			//if (m_SkeletonAnim != null) {
			//	float x, y, w, h = 0f;
			//	float[] vBuffer = new float[0];
			//	m_SkeletonAnim.skeleton.GetBounds(out x, out y, out w, out h, ref vBuffer);

			//	Spine.Animation[] animations = m_SkeletonAnim.Skeleton.Data.Animations.ToArray();

			//	int animLen = animations.Length;

			//	if (animLen > 0) {
			//		m_MainAnim = animations[0].Name;
			//	}

			//	if (animLen > 1) {
			//		m_ExpressionAnim = new string[animLen - 1];

			//		for (int i = 0; i < m_ExpressionAnim.Length; i++) {
			//			m_ExpressionAnim[i] = animations[i + 1].Name;
			//		}
			//	}

			//	m_SkeletonAnim.state.SetEmptyAnimation(0, 0f);
			//	m_SkeletonAnim.state.SetAnimation(0, m_MainAnim, true);
			//}
		}

		void Update()
		{
			if (m_Timer > 0.0f)
			{
				m_Timer -= Time.deltaTime;

				if (m_Timer <= 0.0f)
				{
					m_IsInteractable = true;
				}
			}

			//if (!m_IsInteractable || SFXManager.Instance.HasWaitUntilFinishSound)
			//{
			//	return;
			//}

			//#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
			//        HandleTouchInput();
			//#else
			//		HandleMouseInput();
			//#endif

		}

		public void Reset()
		{
			BoxCollider2D collider2D = GetComponent<BoxCollider2D>();

			if (collider2D != null)
			{
				collider2D.offset = new Vector2(0f, -2f);
				collider2D.size = new Vector2(3f, 10f);
			}

			//if (m_SkeletonAnim == null) {
			//	m_SkeletonAnim = GetComponentInChildren<SkeletonAnimation>();
			//}
		}

		private void PlayExpression()
		{
			//if (SFXManager.Instance.HasWaitUntilFinishSound)
			//{
			//	return;
			//}

			if (!SettingsAPI.SoundsEnabled)
			{
				return;
			}

			//if (m_SkeletonAnim != null) {
			//int idx = GetRandomIndex();

			//if (m_ExpressionAnim[idx] != m_MainAnim) {
			//	SFXManager.Instance.PlayLeaderVoice();
			//	m_SkeletonAnim.state.SetEmptyAnimation(1, 0f);
			//	m_SkeletonAnim.state.AddAnimation(1, m_ExpressionAnim[idx], false, 0);
			//	m_SkeletonAnim.state.AddEmptyAnimation(1, 1f, SFXManager.Instance.CurrentVoiceDuration);
			//}
			//}
			//SFXManager.Instance.PlayLeaderVoice();
		}

		/// <summary>
		/// For instances when expression controller is a child of a UI element.
		/// </summary>
		public void OnButtonPressed()
		{
			//if (m_IsInteractable || SFXManager.Instance.HasWaitUntilFinishSound == false)
			//{
			//	m_IsInteractable = false;
			//	m_Timer = m_Timeout;
			//	PlayExpression();
			//}
		}

		//private void HandleMouseInput()
		//{
		//	if (Input.GetMouseButtonDown(0)) {
		//		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		//		if (m_Collider.OverlapPoint(mousePos) && !IsOverUIObject(Input.mousePosition)) {
		//			m_IsInteractable = false;
		//			m_Timer = m_Timeout;
		//			PlayExpression();
		//		}
		//	}
		//}

		bool IsOverUIObject(Vector2 screenPos)
		{
			EventSystem[] activeSystems = FindObjectsOfType<EventSystem>();
			for (int i = 0; i < activeSystems.Length; i++)
			{
				PointerEventData _eventData = new PointerEventData(activeSystems[i]);
				_eventData.position = screenPos;
				List<RaycastResult> results = new List<RaycastResult>();
				activeSystems[i].RaycastAll(_eventData, results);
				if (results.Count > 0)
				{
					return true;
				}

			}


			return false;
		}

		//private void HandleTouchInput()
		//{
		//	if (Input.touchCount > 0) {
		//		Touch touch = Input.GetTouch(0);

		//		if (touch.phase == TouchPhase.Began) {
		//			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//			if (!IsOverUIObject(touch.position) && m_Collider.OverlapPoint(mousePos)) {

		//				Debug.Log("Pointing over" + IsOverUIObject(Input.mousePosition));
		//				m_IsInteractable = false;
		//				m_Timer = m_Timeout;
		//				PlayExpression();

		//			}
		//		}
		//	}
		//}

		private int GetRandomIndex()
		{
			if (m_ExpressionIndex.Count > 0)
			{
				return m_ExpressionIndex.Pop();
			}

			List<int> nums = new List<int>();

			for (int i = m_ExpressionAnim.Length <= 1 ? 0 : 1; i < m_ExpressionAnim.Length; i++)
			{
				nums.Add(i);
			}

			Random rand = new Random();

			while (nums.Count > 0)
			{
				int idx = rand.Next(0, nums.Count);
				m_ExpressionIndex.Push(nums[idx]);
				nums.RemoveAt(idx);
			}

			return m_ExpressionIndex.Pop();
		}
	}
}
