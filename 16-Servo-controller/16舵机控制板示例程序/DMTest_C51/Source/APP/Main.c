#include <reg52.h>
#include <stdio.h>
#include "DMREG.h"

void UART1_Put_StringL(unsigned char *Str,unsigned char len)
{
	unsigned char i = 0;
//	for (i=0;i<len;i++) UART1_Put_Char(*(Str + i));
	for (i=0;i<len;i++)  SBUF = Str[i];
}

void UART_DM_ReportData(unsigned char data[])
{		
	UART1_Put_StringL(data,10);         
}

//**********动作组及各种舵机的速度角度配置都在“DMREG.h”头文件里面******//
//**********直接把数组名称放在UART_DM_ReportData()函数里面就可以了，例如UART_DM_ReportData(DM0_Speed1_Position_90)********//

void delay_ms(unsigned short i)
{
unsigned short k;
	while(i--)
	for (k=0;k<100;k++); 
}


int main(void)
{  		
	unsigned char i=0;
	TMOD=0x20;		   //用定时器设置串口波特率	   9600 
	TH1=0xfd;
	TL1=0xfd;
	TR1=1;
	TI=1;
	REN=1;          //串口初始化
	SM0=0;
	SM1=1;
	EA=1;           //开启总中断
	ES=1;
	delay_ms(1000);
	
	while(1)
	{	
		UART_DM_ReportData(DM0_Speed1_Position_90);     //设置通道0，速度=1，位置=90度 
		delay_ms(2000);                                 //延时2S    
		UART_DM_ReportData(DM0_Speed3_Position_0);      //设置通道0，速度=3，位置=0度
		delay_ms(2000);
		UART_DM_ReportData(DM_Action0);            //设置动作组0
//		UART_DM_ReportData(DM_Action1);          //设置动作组1
//		UART_DM_ReportData(DM_Action2);          //设置动作组2
//		UART_DM_ReportData(DM_Action3);          //设置动作组3
//		UART_DM_ReportData(DM_Action4);          //设置动作组4
//		UART_DM_ReportData(DM_Action5);          //设置动作组5
//		UART_DM_ReportData(DM_Action6);          //设置动作组6
//		UART_DM_ReportData(DM_Action7);          //设置动作组7        
//		UART_DM_ReportData(DM_Action8);          //设置动作组8         
//		UART_DM_ReportData(DM_Action9);          //设置动作组9
//		UART_DM_ReportData(DM_Action10);         //设置动作组10
//		UART_DM_ReportData(DM_Action11);         //设置动作组11
//		UART_DM_ReportData(DM_Action12);         //设置动作组12
//		UART_DM_ReportData(DM_Action13);         //设置动作组13
//		UART_DM_ReportData(DM_Action14);         //设置动作组14
//		UART_DM_ReportData(DM_Action15);         //设置动作组15
		
	}			
}

void ser() interrupt 4
{
	if (TI) 
		TI=0;  
}
