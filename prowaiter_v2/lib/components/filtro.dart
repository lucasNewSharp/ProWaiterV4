import 'package:flutter/material.dart';

class Filtro extends StatefulWidget {

  
  final Function(String filtro) onFilterChanged;

  Filtro({this.onFilterChanged});

  @override
  _FiltroState createState() => _FiltroState();
}

class _FiltroState extends State<Filtro> {
  TextEditingController filtroController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: TextField(
        onChanged: (value) {
          widget.onFilterChanged(value);
        },
        controller: filtroController,
        decoration: InputDecoration(
            labelText: "Filtro",
            hintText: "Filtro",
            prefixIcon: Icon(Icons.search),
            suffixIcon: IconButton(
              icon: Icon(Icons.clear),
              onPressed: () {
                filtroController.text = "";
                 widget.onFilterChanged("");
              },
            ),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.all(Radius.circular(25.0)))),
      ),
    );
  }
}
