<script setup lang="ts">
import Card from './ui/card/Card.vue';
import type { Organization } from '@/classes/group';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardContent from './ui/card/CardContent.vue';

const props = defineProps<{
    organization:Organization
}>();
</script>

<template>
    <Card>
        <CardHeader>
            <CardTitle>{{props.organization.name}}</CardTitle>
            <CardDescription>{{ props.organization.description }}</CardDescription>
        </CardHeader>
        <CardContent>
            <!-- add groups and halls here (members too?) -->
        </CardContent>
    </Card>
</template>