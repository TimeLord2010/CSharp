using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

public abstract class GUIHandler<T> {

    protected readonly T Pg;
    //private Settings Settings { get => Settings.Default; }

    public GUIHandler(T p) {
        Pg = p;
    }

}