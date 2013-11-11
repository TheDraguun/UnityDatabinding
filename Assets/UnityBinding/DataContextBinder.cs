using UnityEngine;

public class DataContextBinder : Binder {
    public string property;
    IDataContext childDataContext;

    protected override void CreateBindings() {
        if(!string.IsNullOrEmpty(property)) {
            AddBinding<IDataContext>(property, SetDataContext);
        }
    }

    public override IObservable this[string name] {
        get { return childDataContext[name]; }
    }

    public override void Invoke(string functionName) {
        InvokeCore(childDataContext, functionName);
    }

    void SetDataContext(IDataContext value) {
        childDataContext = value;
        BroadcastDataContextChange(childDataContext);
    }

    public override void DataContextChanged(DataContextChangedMessage dataContextMsg) {
        if (isBroadcastingDataContextChange || !gameObject.activeInHierarchy) {
            return;
        }
        var dcb = GetChildOfDataContextBinder();
        if (dcb != null) {
            if (dcb != dataContextMsg.parentContext) {
                return;
            } else {
                //Debug.LogWarning("Child of binder....." + this.gameObject.name + " -- " + dataContextMsg.parentContext);
            }
        }
        DataContext = dataContextMsg.newContext;
        CreateBindings();
        BroadcastDataContextChange(childDataContext);
    }

}