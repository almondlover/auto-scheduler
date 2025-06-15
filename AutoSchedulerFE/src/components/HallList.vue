<script setup lang="ts">
import { ref, watch } from 'vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import Button from './ui/button/Button.vue';
import { useGroupStore } from '@/stores/groupStore';
import HallForm from './HallForm.vue';

const activityStore = useActivityStore();
const { halls } = storeToRefs(activityStore);
const showNewHallModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

watch(currentOrganizationIdx, ()=>{
    halls.value = groupStore.organization(currentOrganizationIdx.value).value?.halls??[];
});
</script>

<template>
    <div v-for="hall in halls" class="flex h-15 items-center justify-between space-x-5">
        <div>
            {{hall.name}}
        </div>
        <Separator orientation="vertical"/>
        <div>
            {{hall.description}}
        </div>
        <Separator orientation="vertical"/>
        <div>
            {{hall.type?.title}}
        </div>
        <Button @click="activityStore.removeHall(hall.id)">Delete</Button>
    </div>
    <Button @click="showNewHallModal=!showNewHallModal">New Hall</Button>
    <HallForm v-show="showNewHallModal"/>
</template>