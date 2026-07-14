import 'package:flutter/material.dart';

class BottomAppBarPadrao extends StatelessWidget {
  final Widget child;

  BottomAppBarPadrao({this.child});

  @override
  Widget build(BuildContext context) {
    return BottomAppBar(
      child: Container(
        padding: const EdgeInsets.fromLTRB(10, 0, 10, 0),
        alignment: Alignment.center,
        height: 70,
        color: Colors.grey[350],
        child: child,
      ),
    );
  }
}
