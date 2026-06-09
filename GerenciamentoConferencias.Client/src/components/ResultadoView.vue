<
<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <h2 class="text-2xl font-bold flex items-center gap-2">
        {{ dados.tipo === 'parse' ? 'Palestras Parseadas' : 'Conferência Organizada' }}
      </h2>
      <div class="flex gap-2">
        <button @click="$emit('nova-entrada', 'texto')" class="btn btn-outline btn-sm">
          Novo Texto
        </button>
        <button @click="$emit('nova-entrada', 'lista')" class="btn btn-outline btn-sm">
          Nova Lista
        </button>
        <button @click="$emit('voltar')" class="btn btn-ghost btn-sm">
          ← Menu
        </button>
      </div>
    </div>

    <!-- Resultado de Conferência -->
    <div class="space-y-6">
      <!-- Visualização em Texto -->
      <div class="card bg-base-100 shadow-xl">
        <div class="card-body">
          <h3 class="card-title text-lg mb-2 flex items-center gap-2">
            <DocumentTextIcon class="size-6" />
            Formato Texto
          </h3>
          <div class="bg-base-300 p-4 rounded-lg font-mono text-sm whitespace-pre-wrap leading-relaxed">
            {{ dados.formatoTexto }}
          </div>
          <button @click="copiarTexto" class="btn btn-sm btn-ghost mt-2">
            {{ copiado ? 'Copiado!' : 'Copiar' }}
          </button>
        </div>
      </div>

      <!-- Visualização em Cards/Lista -->
      <div v-for="trilha in dados.dados.trilhas" :key="trilha.numero" class="card bg-base-100 shadow-xl">
        <div class="card-body">
          <h3 class="card-title text-xl text-primary mb-4 flex items-center gap-2">
            Trilha {{ trilha.numero }}
          </h3>

          <!-- Manhã -->
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2 text-secondary font-semibold">
              <SunIcon class="size-6" />
              Sessão da Manhã ({{ trilha.sessaoManha.duracaoTotal }}min)
            </div>
            <div class="space-y-1">
              <div v-for="p in trilha.sessaoManha.palestras"
                   :key="p.horario"
                   class="flex items-center gap-3 p-2 rounded-lg hover:bg-base-200 transition-colors">
                <span class="font-mono text-primary font-bold w-16">{{ p.horario }}</span>
                <span class="flex-1">{{ p.titulo }}</span>
                <span v-if="p.formatoDuracao != 'relâmpago'" class="badge badge-ghost badge-sm">{{ p.formatoDuracao }}</span>
                <span v-if="p.formatoDuracao == 'relâmpago'" class="badge badge-accent badge-sm"><BoltIcon class="size-4" /></span>
              </div>
            </div>
          </div>

          <!-- Almoço -->
          <div class="divider">
            <span class="badge badge-lg badge-outline">12:00H Almoço</span>
          </div>

          <!-- Tarde -->
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2 text-secondary font-semibold">
              <MoonIcon class="size-6" />
              Sessão da Tarde ({{ trilha.sessaoTarde.duracaoTotal }}min)
            </div>
            <div class="space-y-1">
              <div v-for="p in trilha.sessaoTarde.palestras"
                   :key="p.horario"
                   class="flex items-center gap-3 p-2 rounded-lg hover:bg-base-200 transition-colors">
                <span class="font-mono text-primary font-bold w-16">{{ p.horario }}</span>
                <span class="flex-1">{{ p.titulo }}</span>
                <span v-if="p.formatoDuracao != 'relâmpago'" class="badge badge-ghost badge-sm">{{ p.formatoDuracao }}</span>
                <span v-if="p.formatoDuracao == 'relâmpago'" class="badge badge-accent badge-sm"><BoltIcon class="size-4" /></span>
              </div>
            </div>
          </div>

          <!-- Networking -->
          <div class="alert alert-success mt-2">
            <UserGroupIcon class="size-6" />
            <span class="font-mono font-bold">{{ calcularNetworking(trilha) }} Networking Event</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>import { ref } from 'vue'
import { DocumentTextIcon, SunIcon, MoonIcon, UserGroupIcon, BoltIcon } from '@heroicons/vue/24/solid'

const props = defineProps({
  dados: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['voltar', 'nova-entrada'])

const copiado = ref(false)

const copiarTexto = () => {
  navigator.clipboard.writeText(props.dados.formatoTexto)
  copiado.value = true
  setTimeout(() => copiado.value = false, 2000)
}

const calcularNetworking = (trilha) => {
  const ultima = trilha.sessaoTarde.palestras[trilha.sessaoTarde.palestras.length - 1]
  if (!ultima) return '16:00H'

  const [hora, minuto] = ultima.horario.replace('H', '').split(':').map(Number)
  let h = hora
  let m = minuto + ultima.duracaoMinutos

  while (m >= 60) {
    h++
    m -= 60
  }

  h = Math.max(h, 16)
  return `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}H`
}</script>
