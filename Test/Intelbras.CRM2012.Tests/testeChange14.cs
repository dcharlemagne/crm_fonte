using NUnit.Framework;
using SDKore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class testeChange14 : Base
    {
        [Test]
        public void testeCriacaoParecer()
        {
            this.OrganizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            Domain.Model.Tarefa mTarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, false).BuscaTarefa(new Guid("854B4E70-8E63-E411-A6A3-00155D013D51"));
            new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, false).CriarParecerParaSolicitacao(mTarefa);
        }
    }
}

