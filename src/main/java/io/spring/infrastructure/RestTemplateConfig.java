package io.spring.infrastructure;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.Module;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.databind.deser.std.StdDeserializer;
import com.fasterxml.jackson.databind.module.SimpleModule;
import java.io.IOException;
import java.util.List;
import org.joda.time.DateTime;
import org.joda.time.format.ISODateTimeFormat;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;
import org.springframework.web.client.RestTemplate;

@Configuration
public class RestTemplateConfig {

  @Bean
  public RestTemplate restTemplate(List<Module> modules) {
    ObjectMapper mapper = new ObjectMapper();
    mapper.disable(DeserializationFeature.UNWRAP_ROOT_VALUE);
    mapper.disable(SerializationFeature.WRAP_ROOT_VALUE);
    mapper.disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES);
    for (Module module : modules) {
      mapper.registerModule(module);
    }
    SimpleModule dateTimeDeserializerModule = new SimpleModule();
    dateTimeDeserializerModule.addDeserializer(DateTime.class, new DateTimeDeserializer());
    mapper.registerModule(dateTimeDeserializerModule);
    RestTemplate restTemplate = new RestTemplate();
    MappingJackson2HttpMessageConverter converter = new MappingJackson2HttpMessageConverter();
    converter.setObjectMapper(mapper);
    restTemplate
        .getMessageConverters()
        .removeIf(c -> c instanceof MappingJackson2HttpMessageConverter);
    restTemplate.getMessageConverters().add(converter);
    return restTemplate;
  }

  static class DateTimeDeserializer extends StdDeserializer<DateTime> {
    protected DateTimeDeserializer() {
      super(DateTime.class);
    }

    @Override
    public DateTime deserialize(JsonParser p, DeserializationContext ctxt) throws IOException {
      String text = p.getText();
      if (text == null || text.isEmpty()) {
        return null;
      }
      return ISODateTimeFormat.dateTimeParser().parseDateTime(text);
    }
  }
}
