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

//**********�����鼰���ֶ�����ٶȽǶ����ö��ڡ�DMREG.h��ͷ�ļ�����******//
//**********ֱ�Ӱ��������Ʒ���UART_DM_ReportData()��������Ϳ����ˣ�����UART_DM_ReportData(DM0_Speed1_Position_90)********//

void delay_ms(unsigned short i)
{
unsigned short k;
	while(i--)
	for (k=0;k<100;k++); 
}


int main(void)
{  		
	unsigned char i=0;
	TMOD=0x20;		   //�ö�ʱ�����ô��ڲ�����	   9600 
	TH1=0xfd;
	TL1=0xfd;
	TR1=1;
	TI=1;
	REN=1;          //���ڳ�ʼ��
	SM0=0;
	SM1=1;
	EA=1;           //�������ж�
	ES=1;
	delay_ms(1000);
	
	while(1)
	{	
		UART_DM_ReportData(DM0_Speed1_Position_90);     //����ͨ��0���ٶ�=1��λ��=90�� 
		delay_ms(2000);                                 //��ʱ2S    
		UART_DM_ReportData(DM0_Speed3_Position_0);      //����ͨ��0���ٶ�=3��λ��=0��
		delay_ms(2000);
		UART_DM_ReportData(DM_Action0);            //���ö�����0
//		UART_DM_ReportData(DM_Action1);          //���ö�����1
//		UART_DM_ReportData(DM_Action2);          //���ö�����2
//		UART_DM_ReportData(DM_Action3);          //���ö�����3
//		UART_DM_ReportData(DM_Action4);          //���ö�����4
//		UART_DM_ReportData(DM_Action5);          //���ö�����5
//		UART_DM_ReportData(DM_Action6);          //���ö�����6
//		UART_DM_ReportData(DM_Action7);          //���ö�����7        
//		UART_DM_ReportData(DM_Action8);          //���ö�����8         
//		UART_DM_ReportData(DM_Action9);          //���ö�����9
//		UART_DM_ReportData(DM_Action10);         //���ö�����10
//		UART_DM_ReportData(DM_Action11);         //���ö�����11
//		UART_DM_ReportData(DM_Action12);         //���ö�����12
//		UART_DM_ReportData(DM_Action13);         //���ö�����13
//		UART_DM_ReportData(DM_Action14);         //���ö�����14
//		UART_DM_ReportData(DM_Action15);         //���ö�����15
		
	}			
}

void ser() interrupt 4
{
	if (TI) 
		TI=0;  
}
