using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private Animator _animator;
	private CanvasGroup _canvasGroup;

	public bool IsOpen {
		get {return _animator.GetBool("IsOpen");}
		set {_animator.SetBool("IsOpen", value);}
	}

	public void Awake() {
		_animator = GetComponent<Animator>();
		_canvasGroup = GetComponent<CanvasGroup>();

		// This allows us to take our windows anywhere we want when we're in design mode, but when we hit play, they'll be positioned in the center.
		var rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = new Vector2(0, 0);
	}

	public void Update() {
		// If the animation controller is in the "Open" state, then we enable our canvas group and make it interactable. If not, make it non-interactable.
		if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) {
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
		}
		else {
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
		}
	}

}
