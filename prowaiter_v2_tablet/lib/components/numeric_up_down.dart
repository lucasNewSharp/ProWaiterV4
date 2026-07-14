import 'package:flutter/material.dart';
import '../util/util.dart';

class ChangeEventArgs {  
  int quantidade;
  Object objetoReferenciado;

  ChangeEventArgs({this.quantidade, this.objetoReferenciado});
}

class NumericUpDown extends StatefulWidget {
  final String texto;

  final Object objetoReferenciado;

  final Function(ChangeEventArgs) onChanged;
  

  ///min value user can pick
  final num minValue;

  ///max value user can pick
  final num maxValue;

  /// decimal places required by the counter
  final int decimalPlaces;

  ///Currently selected integer value
  final num initialValue;

  /// if min=0, max=5, step=3, then items will be 0 and 3.
  final num step;

  NumericUpDown(
      {Key key,
      @required this.initialValue,
      @required this.minValue,
      @required this.maxValue,
      @required this.onChanged,
      @required this.decimalPlaces,      
      this.objetoReferenciado,
      this.texto,
      this.step = 1})
      : assert(initialValue != null),
        assert(minValue != null),
        assert(maxValue != null),
        assert(maxValue > minValue),
        assert(initialValue >= minValue && initialValue <= maxValue),
        assert(step > 0),
        super(key: GlobalKey());

  @override
  _NumericUpDownState createState() => _NumericUpDownState();
}

class _NumericUpDownState extends State<NumericUpDown> {
  num selectedValue;  

  @override
  void initState() {
    super.initState();    
    selectedValue = widget.initialValue;    
  }

  void _incrementCounter() {
    setState(() {
      if (selectedValue + widget.step <= widget.maxValue) {
        selectedValue += widget.step;        
        widget.onChanged(_criarEventArgs());
      }
    });
  }

  ChangeEventArgs _criarEventArgs() {
    return ChangeEventArgs(
        quantidade: selectedValue,
        objetoReferenciado: widget.objetoReferenciado
      );
  }

  void _decrementCounter() {
    setState(() {
      if (selectedValue - widget.step >= widget.minValue) {
        selectedValue -= widget.step;        
        widget.onChanged(_criarEventArgs());
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return new Container(
      padding: new EdgeInsets.all(4.0),
      child: new Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        mainAxisSize: MainAxisSize.min,
        children: [
          new SizedBox(
            width: 60,
            height: 40,
            child: FloatingActionButton(
              heroTag: null,
              shape: RoundedRectangleBorder(
                  side: BorderSide.none,
                  borderRadius: BorderRadius.all(
                    Radius.circular(5),
                  )),
              onPressed: _decrementCounter,
              elevation: 2,
              tooltip: 'Incremento',
              child: Icon(Icons.remove),
            ),
          ),
          new Container(
            padding: EdgeInsets.all(4.0),
            child: new Text(
                '${num.parse((selectedValue).toStringAsFixed(widget.decimalPlaces))}',
                style: TextStyle(fontSize: 20)),
          ),
          new SizedBox(
            width: 60,
            height: 40,
            child: FloatingActionButton(
              heroTag: null,
              shape: RoundedRectangleBorder(
                  side: BorderSide.none,
                  borderRadius: BorderRadius.all(
                    Radius.circular(5),
                  )),
              onPressed: _incrementCounter,
              elevation: 2,
              tooltip: 'Incremento',
              child: Icon(Icons.add),
            ),
          ),
          if (!widget.texto.isNullOrWhiteSpace())
            Padding(
              padding: const EdgeInsets.all(6),
              child: Text(
                widget.texto,
                style: TextStyle(fontSize: 18, color: Colors.grey[600]),
              ),
            )
        ],
      ),
    );
  }
}
