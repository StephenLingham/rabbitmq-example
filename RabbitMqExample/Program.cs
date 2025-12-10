using RabbitMqExample;

var publisher = new MessagePublisher();
await publisher.PublishMessageAsync("test-queue", "test message");

// var consumer = new MessageConsumer();
// await consumer.ConsumeMessagesAsync("test-queue");
