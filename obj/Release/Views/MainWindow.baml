   M S B A M L   `   `   `  ����   	VDManager     VDManager.Views.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -      WindowStyle#��[  ����   
Background  �� ImageSource#  [ .���  � RowDefinitions �� $��60 ��� $
��Auto ��� $
��Auto ��� $��6 ��� $��* � � ColumnDefinitions �� $��* �.+��� -    ��Name$ Version��$��2��  VersionTextBlockStyle#��[ ���� *	Version+� $��4��$��0�� �� $
��Auto ��� $
��Auto ��� $��6 ��� $
��Auto ��� $��2 ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��6 ��� $
��Auto � �� $��80 ��� $��* ��� $��80 �.+��� -   $ WelcomeMessage��$��0��$��1��  SpeedTextBlockStyle#��[ ���� *WelcomeMessage+�� -   $ 	AppStatus��$��1��$��1������ *	AppStatus+ �� NotifyOnTargetUpdated . ���� .���  �� Property � �� �� Value$
 Bold"���  � ��$ 16(���  � ��$ Consolas)���  �@��Padding$ 2q���  � ��$
 Auto����  �@��TextAlignment$ Center=���  � ��$ 
0, 0, 0, 5q���  � ��$ Center=���  � ��$ Center=� �� Triggers ��  �� Binding �� *	AppStatus+	 �� Value	 RUNNING.a���  � �� �#55efc4��  �� *	AppStatus+	 STOPPED.a���  � �� �#ee5253�� -   $ InfoStop��$��3��$��1��  DefaultTextBlockStyle#��[ ���� *
InfoStop+�� -   
 ��Name$
 	UseArrows��$��5��$��1��  CheckboxStyle#��[ $��Use arrow keys�� d� 	IsChecked �� *	UseArrows+�� -   $
 UseFKeys��$��6��$��1��#��[ $��
Use F keys�� �� *
UseFKeys+�� -   $
 	UseNumpad��$��7��$��1��#��[ $��Use numpad keys�� �� *	UseNumPad+�� $��9��$��1��$
��Exit��  DefaultButtonStyle#��[ ���� *ExitCommand+