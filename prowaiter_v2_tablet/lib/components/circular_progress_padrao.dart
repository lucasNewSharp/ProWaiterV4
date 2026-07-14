
import 'package:flutter/material.dart';

class CircularProgressPadrao extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    return Center(
            child: Container(
                width: 25,
                height: 25,
                child: CircularProgressIndicator(),                
              )
    );
  }
}