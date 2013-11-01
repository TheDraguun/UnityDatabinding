using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.UnityBinding.NGUI
{
    public class InputBinder : Binder
    {
        UIInput inputBox;

        public string ObservableStringName;
        public bool FireChangeOnEveryKeystroke = false;
        protected Observable<string> inputBinder;

        void Awake() {
            inputBox = GetComponent<UIInput>();
        }

        protected override void CreateBindings() {
            inputBinder = AddBinding<string>(ObservableStringName, value => inputBox.text = value);
        }

        void OnInputChanged() {
            if (FireChangeOnEveryKeystroke)
                inputBinder.SetValue(inputBox.text);
        }

        void OnSubmit() {
            inputBinder.SetValue(inputBox.text);
        }
    }
}
