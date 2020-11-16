using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

public abstract class PageHandler<T> : GUIHandler<T> {

    protected Page InnerPage { get => Pg as Page; }

    public PageHandler(T p) : base(p) {
    }

}