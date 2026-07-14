BEGIN TRAN


UPDATE AspNetUsers SET UserName = 'administrador', Email = 'no-reply@newsharp.com.br', PasswordHash = 'AMYR1raC6sDI9irgIRhpgWU4jupvKeQPQxTfWT8iIDAGeFBRC7ee82T4XGnZnNTdZQ=='																									
	WHERE Id = 'a7c877c2-1030-45e0-9387-851b4b0ea9db'
	

DELETE FROM TBAtribBebidasPedido;
DELETE FROM TBAtribComponentesRefeicao;
DELETE FROM TBAtribComponentesRefeicaoPedido;
DELETE FROM TBAtribRefeicoesPedido;
DELETE FROM TBBebidas;
DELETE FROM TBComponentesRefeicao;
UPDATE TBMesas SET CodUltimoPedido = NULL;
DELETE FROM TBPedidos;
DELETE FROM TBPedidosExternos;
DELETE FROM TBPedidosInternos;
DELETE FROM TBRefeicoesCardapio;
DELETE FROM TBRefeicoes;
DELETE FROM TBClientes
DELETE FROM TBTiposBebida;
DELETE FROM TBTiposRefeicao;
DELETE FROM TBImpressoras;
DELETE FROM TBMesas;
DELETE FROM TBLocaisInternos;

delete from AspNetUsers where Id not in ('a7c877c2-1030-45e0-9387-851b4b0ea9db')

DBCC CHECKIDENT(TBAtribBebidasPedido, RESEED, 0);
DBCC CHECKIDENT(TBAtribRefeicoesPedido, RESEED, 0);
DBCC CHECKIDENT(TBBebidas, RESEED, 0);
DBCC CHECKIDENT(TBClientes, RESEED, 0);
DBCC CHECKIDENT(TBComponentesRefeicao, RESEED, 0);
DBCC CHECKIDENT(TBPedidos, RESEED, 0);
DBCC CHECKIDENT(TBRefeicoes, RESEED, 0);
DBCC CHECKIDENT(TBLocaisInternos, RESEED, 0);


--COMMIT TRAN
ROLLBACK TRAN


