using System;
using UnityEngine;

public class Binder : MonoBehaviour, IDataContext, IInvoke {
    protected IDataContext DataContext;
    protected bool isBroadcastingDataContextChange = false;

    void Awake() {
        DataContext = FindDataContext(transform);
    }

    void Start() {
        CreateBindings();
    }

    public virtual IObservable this[string name] {
        get { return DataContext[name]; }
    }

    public IDataContext GetChildOfDataContextBinder() {
        var currentNode = this.gameObject.transform.parent;

        while (currentNode != null) {
            var dataContext = currentNode.GetComponent(typeof(IDataContext)) as IDataContext;
            if (dataContext != null && dataContext is DataContextBinder) {
                return dataContext;
            }

            currentNode = currentNode.parent;
        }
        return null;
    }

    public virtual void DataContextChanged(DataContextChangedMessage dataContextMsg) {
        if (isBroadcastingDataContextChange || !gameObject.activeInHierarchy) {
            return;
        }
        var dcb = GetChildOfDataContextBinder();
        if (dcb != null) {
            if (dcb != dataContextMsg.parentContext) {
                return;
            }
            else {
                //Debug.LogWarning("Child of binder....." + this.gameObject.name + " -- " + dataContextMsg.parentContext);
            }
        }

        DataContext = dataContextMsg.newContext;
        
        CreateBindings();
        BroadcastDataContextChange(dataContextMsg.newContext);
    }



    protected virtual void BroadcastDataContextChange(IDataContext dc) {
        if (isBroadcastingDataContextChange) {
            return;
        }

        isBroadcastingDataContextChange = true;
        BroadcastMessage("DataContextChanged", new DataContextChangedMessage() {newContext = dc, parentContext = this}, SendMessageOptions.DontRequireReceiver);
        isBroadcastingDataContextChange = false;
    }

    protected virtual void CreateBindings() { }

    IDataContext FindDataContext(Transform start) {
        var currentNode = start;
        
        while(currentNode != null) {
            var dataContext = currentNode.GetComponent(typeof(IDataContext)) as IDataContext;
            if(dataContext != null && dataContext != this) {
                return dataContext;
            }

            currentNode = currentNode.parent;
        }

        return null;
    }

    public virtual void Invoke(string functionName) {
        InvokeCore(DataContext, functionName, null);
    }

    public virtual void Invoke(string functionName, object[] parameters) {
        InvokeCore(DataContext, functionName, parameters);
    }

    protected void InvokeCore(IDataContext dc, string functionName, object[] parameters = null) {
        var invoker = dc as IInvoke;
        if(invoker != null) {
            invoker.Invoke(functionName);
        }

        var behavior = dc as Component;
        if(behavior != null) {
            behavior.SendMessage(functionName, null, SendMessageOptions.DontRequireReceiver);
        }
        else {
            var dcType = dc.GetType();
            var method = dcType.GetMethod(functionName);

            if(method != null) {
                    method.Invoke(dc, parameters);
            }else {
                var parentDC = FindDataContext(transform.parent);
                if(parentDC != null) {
                    InvokeCore(parentDC, functionName, parameters);
                }
            }
        }
    }

    public Observable<T> AddBinding<T>(string name, Action<T> setter) {
        if(DataContext == null) {
            DataContext = FindDataContext(transform);

            if (DataContext == null) {
                return null;
            }
        }

        var ob = DataContext[name];
        if(ob == null) {
            throw new Exception(string.Format("No observable named '{0}' could be found on '{1}'.", name, DataContext));
        }

        setter((T)ob.GetValue());

        ob.ValueChanged += () => {
            setter((T)ob.GetValue());
        };

        return (Observable<T>)ob;
    }

    public class DataContextChangedMessage {
        public IDataContext newContext;
        public IDataContext parentContext;
    }
}