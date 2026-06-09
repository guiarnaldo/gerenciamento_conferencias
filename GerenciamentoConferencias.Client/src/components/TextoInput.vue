<
<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <h2 class="card-title text-2xl flex items-center gap-2">
          Criar Lista de Palestras
        </h2>
        <button @click="$emit('voltar')" class="btn btn-ghost btn-sm">
          ← Voltar
        </button>
      </div>

      <!-- Área de Texto -->
      <div class="form-control">
        <button @click="carregarExemplo" class="btn btn-secondary right">Carregar exemplo</button>

        <fieldset class="fieldset">
          <legend class="fieldset-legend">Escreva a lista de palestras</legend>
          <textarea v-model="texto" class="textarea h-50 w-full"></textarea>
          <div class="label">
            <span class="label-text-alt text-base-content/50">
              Formato: "Título XXmin" ou "Título relâmpago"
            </span>
            <span class="label-text-alt"> - {{ linhasCount }} linhas</span>
          </div>
        </fieldset>
      </div>

      <!-- Ações -->
      <div class="flex gap-3 mt-4">
        <button @click="parsear"
                class="btn btn-primary flex-1"
                :disabled="!texto.trim() || carregando"
                :class="{ 'loading': carregando && acao === 'parse' }">
          <NumberedListIcon class="size-6" />
          Organizar Conferência
        </button>
      </div>

      <!-- Erro -->
      <div v-if="erro" class="alert alert-error mt-4">
        <ExclamationCircleIcon class="size-6" />
        <span>{{ erro }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>import { ref, computed } from 'vue'

import { ExclamationCircleIcon, NumberedListIcon, QueueListIcon } from '@heroicons/vue/24/solid'

const emit = defineEmits(['voltar', 'resultado'])

const texto = ref('')
const carregando = ref(false)
const acao = ref('')
const erro = ref('')

const linhasCount = computed(() => {
  return texto.value.split('\n').filter(l => l.trim()).length
})

const carregarExemplo = () => {
  texto.value = `Como Sobreviver a Reuniões de Sprint 60min
Git Blame: A Arte de Encontrar o Culpado 45min
CSS: Quando tudo é !important 30min
O Mito do Código Legado: É Tudo Legado 45min
Microserviços: De Volta ao Monolito 60min
Rubber Duck Debugging relâmpago
Kubernetes: Orquestrando o Caos 60min
Code Review sem Drama 45min
Sexta-feira em Produção: Um Guia Prático 30min
Refatoração: Ato de Coragem 45min
Stack Overflow Driven Development 30min
Documentação: A Lenda 45min
Serverless: Menos é Mais 60min
Pair Programming: Duas Mentes, Um Teclado 45min
O Poder do Ctrl+C Ctrl+V 5min
DevOps: Não é Sobre Ferramentas 60min
Mensagens de Commit Poéticas 30min
O Síndrome do Impostor no Open Source 45min
Arquitetura Limpa: O Sonho 60min
Debug com Console.log: Técnicas Avançadas 30min
Agile vs Realidade: Um Estudo de Caso 45min
O Último Commit da Sexta 30min
Programação Funcional: Por que Tudo é Função? 60min
Code Smells: Identificando Aromas Suspeitos 45min
O Mundo Antes do Git 30min
APIs RESTful: O Descanso do Desenvolvedor 45min
Dark Mode: Filosofia de Vida 5min
Testes Unitários: Confiança ou Fachada? 60min
O Zen do Terminal 45min
Docker: Funciona na Minha Máquina 60min
Legacy Code: Arqueologia de Software 45min
O Padrão Singleton: Solidão Garantida 30min
Programação Assíncrona: Esperando Nada 60min
Clean Code: A Busca pelo Código Perfeito 45min
O Bug que Ninguém Consegue Reproduzir 30min
Machine Learning: Mágica ou Matemática? 60min
O Código que Escrevi Ontem 5min
Segurança: Não é Paranoia 45min
O Futuro do JavaScript: Ainda JavaScript 30min
Mentoria: Passando o Bastão 45min`
}

const parsear = async () => {
  carregando.value = true
  erro.value = ''

  try {
    const response = await fetch('api/conferencia/organizar', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ texto: texto.value })
    })

    const dados = await response.json()

    if (!response.ok) {
      throw new Error(dados.erro || 'Erro na requisição')
    }

    emit('resultado', {
      tipo: 'conferencia',
      ...dados
    })
  } catch (err) {
    erro.value = err.message
  } finally {
    carregando.value = false
  }
}</script>
