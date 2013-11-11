using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.UnityBinding.NGUI
{
	public class CheckboxBinder : Binder
	{
		UICheckbox checkbox;
		UILabel checkboxLabel;

	    public string ObservableBoolName;
	    public string ObservableStringLabel;
	    protected Observable<bool> checkboxBinder; 

		void Awake() {
			checkbox = GetComponent<UICheckbox>();
		    checkboxLabel = GetComponentInChildren<UILabel>();
		}

		protected override void CreateBindings() {
			checkboxBinder = AddBinding<bool>(ObservableBoolName, value => checkbox.isChecked = value);
            if (!String.IsNullOrEmpty(ObservableStringLabel)) {
                AddBinding<string>(ObservableStringLabel, value => checkboxLabel.text = value);
            }
		}

        void OnActivate(bool isChecked) {
            checkboxBinder.SetValue(isChecked);
        }
	}

}
