<script setup lang="ts">
import { onMounted } from 'vue';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';

const activityStore = useActivityStore();
const { activities } = storeToRefs(activityStore);

onMounted(()=>{
    activityStore.getActivitiesForOrganization(1);
})
</script>

<template>
    <div v-for="activity in activities" class="flex h-5 items-center space-x-5">
        <div>
            {{activity.title}}
        </div>
        <Separator orientation="vertical"/>
         <div>
            {{activity.description}}
        </div>
        <Separator orientation="vertical"/>
        <RouterLink :to="`/activities/${activity.id}`">Go to page</RouterLink>
    </div>
</template>